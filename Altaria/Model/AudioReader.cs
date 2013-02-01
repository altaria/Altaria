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

namespace Altaria.Model
{
    public class AudioReader
    {
        static string[] audioExtensions = {
        ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA"
        };
        public string name { get; set;}
        public string path { get; set;}

        public AudioReader(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
        

        public static bool isAudioFile(string path)
        {
            return -1 != Array.IndexOf(audioExtensions, Path.GetExtension(path).ToUpperInvariant());
        }

        public void readAudio()
        {
            string debugstring = "";
            string inputstr = "";
            byte[] file = fileReader();
            double[] x = new double[file.Length-44];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = file[i+44] / 32768.0;
            //    debugstring = debugstring + x[i].ToString() + " ";
            //    inputstr = inputstr + file[i + 44].ToString() + " " ;
            }
            //Debug.WriteLine("input " + inputstr);
            //Debug.WriteLine("x " + debugstring);
            debugstring = "";

            Complex[] samples = new Complex[x.Length];
            for (int j = 0; j < x.Length; j++)
            {
                samples[j] = x[j];
            //    debugstring = debugstring + samples[j].ToString() + " ";
            }
            //Debug.WriteLine("samples " + debugstring);
            debugstring = "";

            Transform.FourierForward(samples);
            double[] y = new double[samples.Length];
            for (int k = 0; k < samples.Length; k++)
            {
                y[k] = samples[k].Real;
            //    debugstring = debugstring + y[k].ToString() + " ";

            }
            //Debug.WriteLine("y " + debugstring);
            debugstring = "";

            Transform.FourierInverse(samples);
            double[] z = new double[samples.Length];
            for (int k = 0; k < samples.Length; k++)
            {
                z[k] = samples[k].Real;
                debugstring = debugstring + z[k].ToString() + " ";

            }
            Debug.WriteLine("z " + debugstring);
            debugstring = "";

            byte[] output = new byte[file.Length];
            for (int i = 0; i < z.Length; i++)
            {
                Int16 byteint = Convert.ToInt16(z[i] * 32768.0);
                output[i + 44] = (byte)byteint;
                debugstring = debugstring + output[i].ToString() + " ";
                inputstr = inputstr + byteint.ToString() + " ";
            }
            Debug.WriteLine("input " + inputstr);
            Debug.WriteLine("output" + debugstring);
            debugstring = "";

            for (int i = 0; i < 43; i++)
            {
                output[i] = file[i];
            }
            
            Debug.WriteLine("beforewrite");
            ByteArrayToFile("C:\\Users\\Cyrus\\Documents\\GitHub\\Altaria\\Altaria\\Audio\\77777777.wav", output);

            Debug.WriteLine("afterwrite");
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
                Debug.WriteLine("return success");
                return file;
            }
            catch { }
            Debug.WriteLine("return null");
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
            //strOutputInfo.Append("Audio Bitrate= " + oMediaInfo.Audio_Bitrate + "<br />");
            strOutputInfo.Append("Audio Sampling Rate= " + oMediaInfo.Sampling_Rate + "<br />");
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