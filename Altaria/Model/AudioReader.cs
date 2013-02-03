using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MathNet.Numerics;
using System.IO;
using System.Text;
using All4DotNet;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Diagnostics;
using MoreLinq;

namespace Altaria.Model
{
    public class AudioReader
    {
        static string[] audioExtensions = {
        ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA"
        };
        public string name { get; set;}
        public string path { get; set; }
        public int sampleRate { get; set; }
        public int bitRate { get; set;}
        public byte[] subchunksize { get; set; }

        public AudioReader(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
        

        public static bool isAudioFile(string path)
        {
            return -1 != Array.IndexOf(audioExtensions, Path.GetExtension(path).ToUpperInvariant());
        }

        public int audioHeaderLength()
        {
            string extension = Path.GetExtension(name).ToUpperInvariant();
            if (extension.Equals(audioExtensions[0]))
            {
                return BitConverter.ToInt32(subchunksize,0)+29;
            }
            else if (extension.Equals(audioExtensions[2]))
            {
                return 21;
            }
            else if (extension.Equals(audioExtensions[4]))
            {
                return (144* bitRate/sampleRate)+30;
            }
            else return 0;
        }

        public void readAudio(int userid, int frameLength)
        {
            string debugstring = "";
            string inputstr = "";
            byte[] file = fileReader();
            subchunksize = new byte[4];
            subchunksize[0] = file[16];
            subchunksize[1] = file[17];
            subchunksize[2] = file[18];
            subchunksize[3] = file[19];

            //converting file into double[]

            int headerLength = audioHeaderLength();
            Debug.WriteLine("Header Length : " + headerLength);
            double[] x = new double[file.Length - headerLength];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = file[i + headerLength] / 32768.0;
            
            }
            //convert double[] into complex[]
            Complex[] samples = new Complex[x.Length];
            for (int j = 0; j < x.Length; j++)
            {
                samples[j] = x[j];
            }
            Debug.WriteLine("samples.Length " + samples.Length);
            //split complex[] into frames

            var frames = samples.Batch(frameLength);
            int frameCount = 0;

            int remainder = 0;
            List<Complex[]> complexframes = new List<Complex[]>();
            foreach (var batch in frames)
            {
                remainder = batch.Count();
                frameCount++;
                foreach (var frame in batch)
                {
                }
                complexframes.Add(batch.ToArray());
            }


            foreach (var frame in complexframes)
            {
                Transform.FourierForward(frame);
            }

            KeyGen key = new KeyGen(userid, frameLength, frameCount);
            complexframes = key.embedWatermark(complexframes);
            //foreach (Complex[] frame in frames)
            //{
            //    
            //
            //}
            Debug.WriteLine("Before Fourier Transform Inverse");
            // foreach (Complex[] frame in frames)
            //{
            //    Transform.FourierInverse(frame);
            //
            //}

            byte[] output = new byte[((frameCount-1) * frameLength) + remainder + headerLength];
            int bytecounter = 0;
            foreach (var frame in complexframes)
            {
                foreach (var singlecomplex in frame)
                {
                    output[bytecounter + headerLength] = (byte)(Convert.ToInt16(singlecomplex.Real * 32768.0));
                    bytecounter++;
                }
            }
            debugstring = "";

            for (int i = 0; i < headerLength-1; i++)
            {
                output[i] = file[i];
            }

            Debug.WriteLine("Writing new file : " + path + "new_" + name);
            ByteArrayToFile(path + "new_"+name, output);

            Debug.WriteLine("File Successfully written");
        }
        static byte[] GetBytes(double[] values)
        {
            return values.SelectMany(value => BitConverter.GetBytes(value)).ToArray();
        }
        public byte[] fileReader()
        {
            Debug.WriteLine(path + name);
            FileStream fs = File.OpenRead(path+name);
            try
            {
                byte[] file = new byte[fs.Length];
                fs.Read(file, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                Debug.WriteLine("File Converted to Bytes, returned.");
                return file;
            }
            catch { }
            Debug.WriteLine("File was empty, returned");
            return null;
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        public string audioInfo()
        {

            string strRootPath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath);
            MediaManagerPro oMediaManagerPro = new MediaManagerPro();
            oMediaManagerPro.FFMPEG_Path = HttpContext.Current.Server.MapPath("~\\ffmpeg\\ffmpeg.exe");
            oMediaManagerPro.SourceFile_Path = path;
            oMediaManagerPro.SourceFile_Name = name;
            MediaInfo oMediaInfo = oMediaManagerPro.Get_MediaInfo();

            #region Print Output Information
            StringBuilder strOutputInfo = new StringBuilder();
            strOutputInfo.Append("Audio Name= " + name + "<br />");
            strOutputInfo.Append("Audio Codec= " + oMediaInfo.Audio_Codec + "<br />");
            //strOutputInfo.Append("Video Codec= " + oMediaInfo.Video_Codec + "<br />");
            //strOutputInfo.Append("Video Bitrate= " + oMediaInfo.Video_Bitrate + "<br />");
            strOutputInfo.Append("Audio Bitrate= " + oMediaInfo.Audio_Bitrate + "<br />");
            strOutputInfo.Append("Audio Sampling Rate= " + oMediaInfo.Sampling_Rate + "<br />");
            
            String strsampleRate = oMediaInfo.Sampling_Rate;
            String strbitRate = oMediaInfo.Audio_Bitrate;
            String[] tokens = strsampleRate.Split(new[] { ' ' });
            String[] tokens2 = strbitRate.Split(new[] { ' ' });
            sampleRate = Int32.Parse(tokens[1]);
            bitRate = Int32.Parse(tokens2[1])*1000;
            strOutputInfo.Append("Audio Channel= " + oMediaInfo.Channel + "<br />");
            //strOutputInfo.Append("Video Frame_Rate= " + oMediaInfo.Frame_Rate + "<br />");
            strOutputInfo.Append("Audio Duration= " + oMediaInfo.Duration + "<br />");
            strOutputInfo.Append("Audio Duration in Seconds= " + oMediaInfo.Duration_Sec + "<br />");
            //strOutputInfo.Append("Width= " + oMediaInfo.Width + "<br />");
            //strOutputInfo.Append("Height= " + oMediaInfo.Height + "<br />");
            #endregion

            return strOutputInfo.ToString();
        }
        public void testloop()
        {
            List<int[]> frames = new List<int[]>();
            int p = 0;
            int frameLength = 5;
            int[] fibarray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            int remainder = fibarray.Length % frameLength;
            int[] singleFrame = new int[frameLength];

            var arrays = fibarray.Batch(5);

            foreach (var batch in arrays)
                {
                    foreach (var item in batch)
                    {
                        Debug.WriteLine(item);
                    } 
                }

            //for (int i = 0; i < fibarray.Length; i++)
            //{

            //    if (p >= frameLength)
            //    {

            //        Debug.WriteLine("Frame Added");
            //        frames.Add(singleFrame);

            //        Debug.WriteLine("frames[0] : " + frames[0][0] + frames[0][1] + frames[0][2] + frames[0][3] + frames[0][4]);
            //        p = 0;
            //    }
            //    singleFrame[p] = fibarray[i];
            //    Debug.WriteLine("singleFrame[p] : " + p + " = fibarray[i] : " + i);
            //    p++;
            //}
            //if (remainder != 0)
            //{
            //    int[] lastFrame = new int[remainder];

            //    for (int i = 0; i < lastFrame.Length; i++)
            //    {
            //        Debug.WriteLine("lastFrame[p] : " + i + " = singleFrame[i] : " + i);
            //        lastFrame[i] = singleFrame[i];
            //    }
            //    frames.Add(lastFrame);
            //}
            //else
            //{
            //    remainder = frameLength;
            //    frames.Add(singleFrame);
            //}
            //Debug.WriteLine("frames[0] : " + frames[0][0] + frames[0][1] + frames[0][2] + frames[0][3] + frames[0][4]);
            //Debug.WriteLine("frames[1] : " + frames[1][0] + frames[1][1] + frames[1][2] + frames[1][3] + frames[1][4]);
            //Debug.WriteLine("frames[2] : " + frames[2][0] + frames[2][1] + frames[2][2] + frames[2][3] + frames[2][4]);

            //int bytecounter = 0;

            //foreach (int[] frame in frames)
            //{
            //    foreach (int singlebyte in frame)
            //    {
            //        Debug.WriteLine("number : " + singlebyte);
            //        bytecounter++;
            //    }
            //}
        }
    }
}