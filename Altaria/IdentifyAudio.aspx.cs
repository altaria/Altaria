using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Altaria.Model;
using System.IO;
using NAudio.Wave;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Altaria
{
    public partial class IdentifyAudio : System.Web.UI.Page
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Digest
        {
            public string id;
            public UIntPtr coeffs;
            public int size;
        }

        [DllImport(@"C:\temp\requiredFiles\codegen\windows\Release\codegen.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getFingerPrint(string filename, int start_offset, int duration);

        String savePath = @"C:\temp\uploads\";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void scanAudio_Click(object sender, EventArgs e)
        {
            addSongResult.Visible = false;
            waiting.Visible = false;

            if (uploadedfile.HasFile)
            {
                string displayedResult = "";
                ArrayList listOfResults = new ArrayList();

                // Upload the file to server
                String fileName = uploadedfile.FileName;
                savePath += fileName;
                uploadedfile.SaveAs(savePath);
                
                string fileExtension = Path.GetExtension(savePath);

                if (!fileExtension.Equals(".mp3"))
                {
                    savePath = convertVideoToAudio(fileName);
                    savePath = savePath.Trim();
                }

                SongRetrieval sr = new SongRetrieval();
                sr.retrieveAllHash();
                ArrayList hashValue = sr.getSongHashes();

                double songLength = Math.Round((getMediaDuration(savePath) / 4) / 60, 2);

                for (int b = 0; b < (getMediaDuration(savePath) / 4 - 2); b += 30)
                //for(int b = 0; b < 30; b+=30)
                {
                    // Analyze the files (input file)
                    string filename = savePath;
                    IntPtr fingerprint = getFingerPrint(filename, b, 30);
                    string analysisResults = Marshal.PtrToStringAnsi(fingerprint);

                    SongInfo song = JsonHelper.JsonDeserializer<SongInfo>(analysisResults);

                    double[,] clientCode = analyzeCodegen(song.code);

                    // Analyze input file with hashes obtained from database
                    double[,] masterCode = null;
                    ArrayList matchingEvents = new ArrayList(); //List of events that gives the greatest match (value stored is the index that gives the most number of matches)

                    int maxConsecutiveCount = 9;
                    int previousCount = 0;
                    int currentCount = 0;

                    // Identify events that matches
                    for (int i = 0; i < hashValue.Count; i++)
                    {
                        masterCode = analyzeCodegen(hashValue[i].ToString());
                        int[] comparisonResult = compareClientMasterWithoutTimeOffset(clientCode, masterCode);
                        currentCount = comparisonResult[1];

                        // Compares first criteria - Consecutive hashes
                        if (comparisonResult[0] == maxConsecutiveCount)
                        {
                            matchingEvents.Add(i);
                        }
                        else if (comparisonResult[0] > maxConsecutiveCount)
                        {
                            matchingEvents.Clear();
                            matchingEvents.Add(i);

                            maxConsecutiveCount = comparisonResult[0];
                        }

                        // Compares second criteria - Total count
                        if (matchingEvents.Count > 1)
                        {
                            if (currentCount > previousCount)
                            {
                                matchingEvents.Clear();
                                matchingEvents.Add(i);
                            }
                            else
                                matchingEvents.RemoveAt(1);
                        }

                        if (comparisonResult[0] >= maxConsecutiveCount)
                            previousCount = currentCount;
                    }

                    ArrayList songIdList = (ArrayList)sr.getSongIdList();
                    int[] songId = new int[matchingEvents.Count];

                    for (int i = 0; i < matchingEvents.Count; i++)
                    {
                        songId[i] = (int)songIdList[(int)matchingEvents[i]];
                    }

                    // Identify the song
                    string title = "Unidentified";
                    SongRetrieval identifiedSong = null;

                    for (int i = 0; i < songId.Length; i++)
                    {
                        SongRetrieval songHash = new SongRetrieval((int)songId[i], true);

                        if (i == 0)
                        {
                            identifiedSong = songHash.retrieveSongFromSongId();
                            title = identifiedSong.getSongTitle();
                        }
                        else
                        {
                            if (!title.Equals(songHash.retrieveSongFromSongId().getSongTitle()))
                                title = "Unidentified";
                        }
                    }

                    // Identifying the accuracy of the results
                    int accuracy = 0;

                    if (maxConsecutiveCount > 700)
                        accuracy = 100;
                    else if (maxConsecutiveCount > 500)
                        accuracy = 80;
                    else if (maxConsecutiveCount > 300)
                        accuracy = 50;
                    else if (maxConsecutiveCount > 100)
                        accuracy = 30;
                    else if (maxConsecutiveCount > 50)
                        accuracy = 20;
                    else if (title.Equals("Unidentified"))
                        accuracy = 0;
                    else
                        accuracy = 10;

                    // Identifying whether the identified song matches the meta data
                    string misc = "";
                    if (song.metadata.title.Equals(""))
                    {
                        misc = "This song has no file properties attached.";
                    }
                    else if (title.Equals(song.metadata.title))
                    {
                        if (accuracy < 90)
                            accuracy += 10;
                        misc = "Song identified matches file properties of the file sent for analysis.";
                    }
                    else if (title.Equals("Unidentified"))
                    {
                        misc = "Song could not be identified. This song might not exist in our database.";
                    }
                    else
                    {
                        if (accuracy <= 10)
                            accuracy -= 10;

                        if (accuracy > 50)
                            misc = "The file properties might not be labelled correctly as it does not match the song identifed by us!";
                        else
                        {
                            misc = "Song identified might not be accurate as the desired song might not be in our database or large changes has been done to the file";
                        }
                    }

                    displayedResult = "File Analyzed: " + fileName + "<br/><br/>File Properties<br/>============<br/>" +
                                        "Title: " + song.metadata.title + "<br/>" +
                                        "Album Artist: " + song.metadata.artist + "<br/>" +
                                        "Album: " + song.metadata.release + "<br/>" +
                                        "Length: " + songLength + " minutes<br/>" +
                                        "Genre: " + song.metadata.genre + "<br/>" +
                                        "Bitrate: " + song.metadata.bitrate + " kbps<br/><br/>" +
                                        "Audio Identification Results" + "<br/>=====================<br/>";

                    // If the song can be identified and the accuracy threshold is set to 20%, it will be added to be displayed as results
                    if (identifiedSong != null && accuracy > 20)
                    {
                        SongIdentification si = new SongIdentification(identifiedSong, accuracy, maxConsecutiveCount, misc);
                        listOfResults.Add(si);
                    }
                }

                string sumTitle = "";
                
                for (int j = 0; j < listOfResults.Count; j++)
                {
                    SongIdentification mainObj = (SongIdentification)listOfResults[j];
                    SongRetrieval song = mainObj.getIdentifiedSong();

                    if (!sumTitle.Contains(song.getSongTitle()))
                    {
                        if (!sumTitle.Equals("") && mainObj.getAccuracy() > 10)
                        {
                            sumTitle = sumTitle + ", " + song.getSongTitle();
                        }
                        else
                        {
                            sumTitle = song.getSongTitle();
                        }

                        displayedResult = displayedResult + "Title: " + song.getSongTitle() + "<br/>" +
                                        "Album Artist: " + song.getArtistName() + "<br/>" +
                                        "Album: " + song.getAlbumName() + "<br/>" +
                                        "Total Matching Consecutive Hashes: " + mainObj.getMaxConsecutiveHashes() + "<br/>" +
                                        "Accuracy Level: " + mainObj.getAccuracy() + " %<br/>" +
                                        "Matching of File Properties: " + mainObj.getMisc() + "<br/><br/>";
                    }
                }

                if (!sumTitle.Equals(""))
                {
                    result.Text = "<b>File Analyzed: " + fileName + "<br/>Song Identified: " + sumTitle + "</b>";
                    ingestFingerprint.Visible = false;

                    displayedResult = displayedResult + "Conclusion<br/>=============<br/>" +
                                    "Song Identified: " + sumTitle;
                }
                else
                {
                    result.Text = "<b>File Analyzed: " + fileName + "<br/>Song could not be identified</b><br/><br/>To contribute, you can choose to add this song into our database.";
                    ingestFingerprint.Visible = true;
                    pathName.Text = savePath;

                    displayedResult = displayedResult + "Title: Unidentified" + "<br/>" +
                                        "Album Artist: Unidentified" + "<br/>" +
                                        "Album: Unidentified" + "<br/><br/>" +
                                        "Conclusion<br/>=========<br/>" +
                                        "Song could not be identified";
                }

                exportedResults.Text = displayedResult;

                waiting.Text = "Analysis Completed!";

                result.Visible = true;
                exportToPdf.Enabled = true;
                exportToPdf.Visible = true;
            }
            else
            {
                waiting.Visible = true;
                waiting.Text = "No file has been selected for analysis";
            }
        }

        /* Split the results that is return into individual elements
            eg: [time, hash] 
                [time, hash] */
        public static double[,] analyzeCodegen(string clientCode)
        {
            string[] parts = clientCode.Split(';');
            double[,] value = new double[parts.Length-1,2];

            try
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    string[] splitResult = parts[i].Split(' ');

                    value[i, 0] = Convert.ToDouble(splitResult[0]);
                    value[i, 1] = Convert.ToDouble(splitResult[2]);
                }
            }
            catch (Exception e)
            {
            }

            return value;
        }

        /* Compares the generated time hash pairs of the client and master based on the time offsets
            client -> User input audio file
            master -> Database audio file
            result[0] = Total Number of Events that Match
            result[1] = Total Number of Events */
        public static double[] compareClientMasterWithTimeOffset(double[,] clientCode, double[,] masterCode)
        {
            double[] result = new double[2];
            double matchedEvents = 0;
            double totalEvents = 0;
            int lastRecordedValue = (int) clientCode[0, 0];
            //int lastRecordedValue = 0;
            bool clientStay = false;
            int masterValue = 0;
            int initialValue = (int)clientCode[0, 0];

            if (clientCode[0, 0] > masterCode[0, 0])
            {
                initialValue = (int)masterCode[0, 0];
                lastRecordedValue = (int)masterCode[0, 0];
            }
            else
            {
                initialValue = (int)clientCode[0, 0];
                lastRecordedValue = (int)clientCode[0, 0];
            }

            bool newSubband = false;

            for (int i = 0; i < clientCode.Length / 2; i = i + 6)
            {
                if (clientStay)
                {
                    masterValue = i;
                    i = i - 6;
                    clientStay = false;
                }

                int clientTimeValue = (int)clientCode[i, 0];
                int masterTimeValue = 0;
                
                if (masterValue < (masterCode.Length/2))
                    masterTimeValue = (int)masterCode[masterValue, 0];

                bool timeMatched = true;

                if (!newSubband && initialValue == clientTimeValue)
                {
                    newSubband = true;
                    lastRecordedValue = clientTimeValue;
                }
                else
                    newSubband = false;

                while (clientTimeValue != masterTimeValue)
                {
                    if (masterTimeValue < lastRecordedValue)
                    {
                        timeMatched = false;
                        break;
                    }
                    else if (newSubband && masterTimeValue > clientTimeValue)
                    {
                        clientStay = true;
                        timeMatched = false;
                        break;
                    }
                    else if (masterTimeValue > clientTimeValue)
                    {
                        timeMatched = false;
                        break;
                    }
                    else
                    {
                        masterValue += 6;
                        if (masterValue >= (masterCode.Length / 2))
                        {
                            timeMatched = false;
                            break;
                        }
                        else
                        {
                            clientTimeValue = (int)clientCode[i, 0];
                            masterTimeValue = (int)masterCode[masterValue, 0];
                        }
                    }
                }

                if (timeMatched)
                {
                    if (clientTimeValue == masterTimeValue && i < (clientCode.Length/2) && masterValue < (masterCode.Length/2))
                    {
                        int clientValue = i;
                        for (int j = 0; j < 6; j++)
                        {
                            if (clientCode[clientValue, 1] == masterCode[masterValue, 1])
                                matchedEvents++;

                            clientValue++;
                            masterValue++;
                            
                            totalEvents++;
                        }
                    }

                    lastRecordedValue = clientTimeValue;
                }
            }

            result[0] = matchedEvents;
            result[1] = totalEvents;

            Debug.WriteLine("Number of Matched Events: " + matchedEvents);
            Debug.WriteLine("Number of Events: " + totalEvents);
            return result;
        }

        /* From the specified file name, the media duration of the song is retrieved
         * Allows the calculation of the entire song for ingesting fingerprints and song details */
        public static double getMediaDuration(string fileName)
        {
            double duration = 0.0;

            using (FileStream fs = File.OpenRead(fileName))
            {
                Mp3Frame frame = Mp3Frame.LoadFromStream(fs);
                while (frame != null)
                {
                    if (frame.ChannelMode == ChannelMode.Mono)
                        duration += (double)frame.SampleCount * 2.0 / (double)frame.SampleRate;
                    else
                        duration += (double)frame.SampleCount * 4.0 / (double)frame.SampleRate;

                    frame = Mp3Frame.LoadFromStream(fs);
                }
            }
            return duration;
        }

        protected void ingestFingerprint_Click(object sender, EventArgs e)
        {
            bool success = false;

            if (!pathName.Text.Equals(""))
            {
                success = addNewFingerprint(pathName.Text);
                if (success)
                {
                    addSongResult.Text = "Song has been added successfully. Thank you for your contribution";
                    ingestFingerprint.Enabled = false;
                }
                else
                    addSongResult.Text = "Required meta data fields has been left empty. Please fill in associated meta data";
            }
            else
            {
                addSongResult.Text = "No file has been selected for addition into our database.";
            }

            addSongResult.Visible = true;
        }

        /* Add new fingerprint into the database according to the specified file name
         * */
        public bool addNewFingerprint(string fileName)
        {
            int songDuration = (int)(getMediaDuration(fileName) / 4) - 2; // In seconds
            double[] comparisonResult = new double[2];
            Artist artist = null;
            Album album = null;
            SongRetrieval sr = null;
            int songId = -1;

            for (int i = 0; i < songDuration; i = i + 30)
            {
                IntPtr fingerprint2 = getFingerPrint(fileName, i, 60);
                string analysisResults2 = Marshal.PtrToStringAnsi(fingerprint2);

                SongInfo song2 = JsonHelper.JsonDeserializer<SongInfo>(analysisResults2);

                if (song2.metadata.title.Equals("") || song2.metadata.artist.Equals("") || song2.metadata.release.Equals(""))
                    return false;

                if (i == 0)
                {
                    artist = new Artist(song2.metadata.artist);
                    artist.InsertArtist();

                    album = new Album(song2.metadata.release);
                    album.InsertAlbum();

                    sr = new SongRetrieval(artist.retrieveArtistId(), album.retrieveAlbumId(), song2.metadata.title, song2.metadata.bitrate);
                    sr.InsertSong();
                    songId = sr.retrieveLatestSongId();

                    sr = null;
                    sr = new SongRetrieval(songId, song2.code, i);
                    sr.InsertSongHash();
                }
                else
                {
                    sr.setSongHash(song2.code);
                    sr.setSongOffset(i);
                    sr.InsertSongHash();
                }
            }

            return true;
        }

        /* Compares the song onset without specifying the time
         * Window period of 5 hashes are calculated and given
         * int[] format -> [0] = total consecutive of 5 hash pairs (criteria 1)
         *                 [1] = total matching count of the entire hash (criteria 2)
         * */
        public static int[] compareClientMasterWithoutTimeOffset(double[,] clientCode, double[,] masterCode)
        {
            int num = 0;
            int numOfConsecutive = 0;
            int value = 0;
            int[] result = new int[2];

            for (int i = 0; i < clientCode.Length / 2; i++)
            {
                double clientValue = clientCode[i, 1];
                for (int j = 0; j < masterCode.Length / 2; j++)
                {
                    double masterValue = masterCode[j, 1];
                    if (masterValue == clientValue)
                    {
                        num++;
                        value++;

                        try
                        {
                            // Testing window period of 5 consecutive hashes
                            for (int a = 1; a < 5; a++)
                            {
                                if (clientCode[i + a, 1] == masterCode[j + a, 1])
                                {
                                    num++;
                                }
                                else
                                {
                                    num = 0;
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            num = 0;
                        }

                        if (num >= 5)
                        {
                            numOfConsecutive++;
                        }
                    }
                    else
                        num = 0;
                }
            }

            result[0] = numOfConsecutive;
            result[1] = value;

            return result;
        }

        protected void exportToPdf_Click(object sender, EventArgs e)
        {
            string text = "\n" + exportedResults.Text;
            text = text.Replace("<br/>", "\n");

            Document doc = new Document(PageSize.A4.Rotate());
            string filePath = Server.MapPath("Analysis Results.pdf");
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();
            Paragraph heading = new Paragraph("Audio Identification Results", new Font(Font.FontFamily.HELVETICA, 18f, Font.BOLD));
            doc.Add(heading);
            doc.Add(new Paragraph(text));
            
            doc.Close();

            Response.ContentType = "Application/pdf";
            Response.WriteFile(filePath);
            Response.End();
        }

        public string convertVideoToAudio(string fileName)
        {
            string ffmpegPath = Path.GetFullPath("cmd.exe");
            string oldFile = @"C:\temp\uploads\" + fileName;
            string newFile = @" C:\temp\uploads\" + fileName.Substring(0, fileName.LastIndexOf(".")) + ".mp3";

            string command = @"/c ffmpeg -i """ + oldFile + @"""" + newFile + @"""";

            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = ffmpegPath;
                proc.StartInfo.Arguments = command;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = false;
                proc.StartInfo.CreateNoWindow = false;

                proc.Start();
                proc.WaitForExit();
                
                return newFile;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}