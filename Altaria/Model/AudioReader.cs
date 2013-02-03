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
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

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
        public int bytestodata { get; set; }
        public readonly byte[] datasection = new byte[] { 100, 97, 116, 97 };

        public AudioReader(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
        public AudioReader()
        {
        }
        

        public static bool isAudioFile(string newpath)
        {
            return -1 != Array.IndexOf(audioExtensions, Path.GetExtension(newpath).ToUpperInvariant());
        }

        public int audioHeaderLength()
        {
            string extension = Path.GetExtension(name).ToUpperInvariant();
            if (extension.Equals(audioExtensions[0]))
            {
                return bytestodata+9;
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

        public static string verifyWatermark(List<Complex[]> complexframes1, List<Complex[]> complexframes2, int userid, string filename, string watermarkpath)
        {
            int shorter;
            if (complexframes1[0].Length >= complexframes2[0].Length)
            { shorter = complexframes2[0].Length; }
            else { shorter = complexframes1[0].Length; }

            string storedkey = "";
            string name = "";
            string regno = "";
            string address = "";
            string contact = "";
            string keyinfo = "";
            string datakey = "";
            DBConnect connection = new DBConnect();

            string query = "SELECT keydata.keydatadata, userdata.userdataname, userdata.userdataregno, userdata.userdataaddress, userdata.userdatacontact, keydata.keydatainfo, keydata.keydatakey FROM keydata INNER JOIN userdata ON keydata.iduserdata = userdata.iduserdata WHERE (keydata.iduserdata = @userid) AND (keydata.keysongname = @songname)";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@songname", filename);
                MySqlDataReader Reader = cmd.ExecuteReader();
                if (!Reader.HasRows) return "No Matches found in Database!";
                while (Reader.Read())
                {
                    name = KeyGen.GetDBString("userdataname", Reader);
                    regno = KeyGen.GetDBString("userdataregno", Reader);
                    storedkey = KeyGen.GetDBString("keydatadata", Reader);
                    address = KeyGen.GetDBString("userdataaddress", Reader);
                    contact = KeyGen.GetDBString("userdatacontact", Reader);
                    keyinfo = KeyGen.GetDBString("keydatainfo", Reader);
                    datakey = KeyGen.GetDBString("keydatakey", Reader);
                }
                Reader.Close();
                connection.CloseConnection();

            }
            catch (Exception ex)
            {
            }

            if (!storedkey.Equals("") && (storedkey != null))
            {
                double[] d = KeyGen.StringToDoubleArr(storedkey);


                double[] weight = new double[5];
                double avg = 0;


                for (int h = 0; h < complexframes1.Count; h++)
                {

                    Complex[] difference = new Complex[shorter];
                    for (int i = 0; i < 5; i++)
                    {
                        difference[i] = complexframes2[h][i] - complexframes1[h][i];
                        weight[i] = difference[i].Real / d[i];
                    }
                    avg = avg + weight.Average();



                }
                avg = avg / complexframes1.Count;


                StringBuilder strOutputInfo = new StringBuilder();
                //if (Math.Abs(avg) >= 0.7 && Math.Abs(avg) <= 1.51)
                Debug.WriteLine(datakey);
                Debug.WriteLine(KeyGen.ToHex(MD5.Create().ComputeHash(File.ReadAllBytes(watermarkpath)), true));
                byte[] computedkeydata = MD5.Create().ComputeHash(File.ReadAllBytes(watermarkpath));
                if (datakey.Equals(KeyGen.ToHex(MD5.Create().ComputeHash(File.ReadAllBytes(watermarkpath)),true)))
                {
                    strOutputInfo.Append("Watermark is Verified<br />");
                    strOutputInfo.Append("Registered User : " + name + "<br />");
                    strOutputInfo.Append("Registration # : " + regno + "<br />");
                    strOutputInfo.Append("User's Contact : " + contact + "<br />");
                    strOutputInfo.Append("User's Address : " + address + "<br />");
                    strOutputInfo.Append("Watermark Info : " + keyinfo + "<br />");
                    return strOutputInfo.ToString();
                }
                else
                {
                    strOutputInfo.Append("Watermark is Rejected");
                    return strOutputInfo.ToString();
                }
            }
            else
            {
                return "No Matches found in Database!";
            }

        }

        public List<Complex[]> readAudio(int userid, int frameLength, int mode)
        {
            //mode1 embed ; mode2 verify
            byte[] file = fileReader();
            bytestodata = KeyGen.SimpleBoyerMooreSearch(file, datasection);

            subchunksize = new byte[4];
            subchunksize[0] = file[16];
            subchunksize[1] = file[17];
            subchunksize[2] = file[18];
            subchunksize[3] = file[19];

            //converting file into double[]
            int headerLength = audioHeaderLength();
            double[] x = ByteToDouble(file, headerLength);
            Debug.WriteLine("Header Length : " + headerLength);
            
            //convert double[] into complex[]
            Complex[] samples = DoubleToComplex(x);
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


            Debug.WriteLine("Before Fourier Transform Forward");
            foreach (var frame in complexframes)
            {
                Transform.FourierForward(frame);
            }



            if (mode == 1 && userid != 0)
            {
                //embedwatermark
                KeyGen key = new KeyGen(userid, frameLength, frameCount, name);
                complexframes = embedWatermark(complexframes, key);



                //convert complex[] to byte[]
                byte[] output = new byte[((frameCount - 1) * frameLength) + remainder + headerLength];
                int bytecounter = 0;
                foreach (var frame in complexframes)
                {
                    foreach (var singlecomplex in frame)
                    {
                        output[bytecounter + headerLength] = (byte)(Convert.ToInt16(singlecomplex.Real * 32768.0));
                        bytecounter++;
                    }
                }

                for (int i = 0; i < headerLength - 1; i++)
                {
                    output[i] = file[i];
                }

                Debug.WriteLine("Writing new file : " + path + "new_" + name);
                ByteArrayToFile(path + "new_" + name, output);

                Debug.WriteLine("File Successfully written");
                

                DBConnect connection = new DBConnect();

                string query = "UPDATE keydata SET keydatakey=@datakey where (iduserdata=@userid) and (keysongname=@name) ";

                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                    cmd.CommandText = query;
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@datakey", KeyGen.ToHex(MD5.Create().ComputeHash(File.ReadAllBytes(path + "new_" + name)),true));
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@name", name);
                    Debug.WriteLine(MD5.Create().ComputeHash(File.ReadAllBytes(path + "new_" + name)));
                 
                    cmd.ExecuteNonQuery();

                    connection.CloseConnection();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SQL INSERT INTO KEYDATA FAILED!");
                }



                return null;
            }
            return complexframes;
        }

       

        

        public List<Complex[]> embedWatermark(List<Complex[]> complexframes, KeyGen key)
        {
            

            Debug.WriteLine("User Key Generated");

            Debug.WriteLine("Before Fourier Transform Inverse");
            complexframes = key.embedWatermark(complexframes);
            return complexframes;

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

        public static double[] ByteToDouble(byte[] file, int headerLength)
        {
            double[] x = new double[file.Length - headerLength];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = file[i + headerLength] / 32768.0;

            }
            return x;
        }
        
        private Complex[] DoubleToComplex(double[] x)
        {
            Complex[] samples = new Complex[x.Length];
            for (int j = 0; j < x.Length; j++)
            {
                samples[j] = x[j];
            }
            return samples;
        }

        private List<Complex[]> ComplexToFrames(Complex[] samples, int frameLength)
        {
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
            return complexframes;
        }

        public byte[] fileReader()
        {
            Debug.WriteLine(path + name);
            FileStream fs = File.OpenRead(path + name);
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

        public static byte[] fileReader(string path)
        {
            Debug.WriteLine(path);
            FileStream fs = File.OpenRead(path);
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
            //strOutputInfo.Append("Audio Bitrate= " + oMediaInfo.Audio_Bitrate + "<br />");
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
        

        
    }
}