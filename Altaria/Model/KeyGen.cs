using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Altaria.Model
{
    public class KeyGen
    {
        int userdata { get; set; }
        int keydata { get; set; }
        Complex[] keydatadata { get; set; }
        double[] keydatadatadouble { get; set; }
        int keyframelength { get; set; }
        string keydatainfo { get; set; }
        string name { get; set; }
        string datakey { get; set; }
        public KeyGen(int userid, int framelength, int framecount, string name)
        {
            Random random = new Random();
            userdata = userid;
            keydatadatadouble = new double[5];
            keyframelength = framelength;
            this.name = name;
            for (int i = 0; i < keydatadatadouble.Length; i++)
            {
                keydatadatadouble[i] = Math.Round(random.NextDouble() / 100, 7);
            }
            keydatainfo = "Key Generated on " + DateTime.Now + " for song : " + name;
        }

        public List<Complex[]> embedWatermark(List<Complex[]> complexframes)
        {

            foreach (var frame in complexframes)
            {
                
                frame[0] = frame[0] + keydatadatadouble[0];
                frame[1] = frame[1] + keydatadatadouble[1];
                frame[2] = frame[2] + keydatadatadouble[2];
                frame[3] = frame[3] + keydatadatadouble[3];
                frame[4] = frame[4] + keydatadatadouble[4];
                Transform.FourierInverse(frame);
                
            }
            keydatainfo = keydatainfo + " | Key Embedded on " + DateTime.Now;

            DBConnect connection = new DBConnect();

            string query = "INSERT INTO keydata(iduserdata,keydatadata,keydatainfo,keyframelength,keysongname) VALUES (@userid, @keydata, @keydatainfo, @keyframelength, @songname)";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@userid", this.userdata);
                //cmd.Parameters.AddWithValue("@keydata", ComplexArrToString(this.keydatadata));
                cmd.Parameters.AddWithValue("@keydata", String.Join(",", keydatadatadouble.Select(p => p.ToString()).ToArray()));
                cmd.Parameters.AddWithValue("@keydatainfo", this.keydatainfo);
                cmd.Parameters.AddWithValue("@keyframelength", this.keyframelength);
                cmd.Parameters.AddWithValue("@songname", this.name);
                cmd.ExecuteNonQuery();

                connection.CloseConnection();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("SQL INSERT INTO KEYDATA FAILED!");
            }

            return complexframes;

        }

        public static string ComplexArrToString(Complex[] complexarr)
        {
            string value = String.Join(",", complexarr.Select(i => i.ToString()).ToArray());
            return value;
        }
        public static double[] StringToDoubleArr(string stringarr)
        {
            Debug.WriteLine(stringarr);
            
            double[] arr = stringarr.Split(',').Select(s => Double.Parse(s)).ToArray();
            double[] complexarr = new double[5];
            for (int i = 0; i < 5;i++)
            {
                complexarr[i] = arr[i];
            }

            return complexarr;
        }

        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
        public static string GetDBString(string SqlFieldName, MySqlDataReader Reader)
        {
            return Reader[SqlFieldName].Equals(DBNull.Value) ? String.Empty : Reader.GetString(SqlFieldName);
        }
        public static int SimpleBoyerMooreSearch(byte[] haystack, byte[] needle)
        {
            int[] lookup = new int[256];
            for (int i = 0; i < lookup.Length; i++) { lookup[i] = needle.Length; }

            for (int i = 0; i < needle.Length; i++)
            {
                lookup[needle[i]] = needle.Length - i - 1;
            }

            int index = needle.Length - 1;
            var lastByte = needle.Last();
            while (index < haystack.Length)
            {
                var checkByte = haystack[index];
                if (haystack[index] == lastByte)
                {
                    bool found = true;
                    for (int j = needle.Length - 2; j >= 0; j--)
                    {
                        if (haystack[index - needle.Length + j + 1] != needle[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return index - needle.Length + 1;
                    else
                        index++;
                }
                else
                {
                    index += lookup[checkByte];
                }
            }
            return -1;
        }

        public static byte[] GetBytes(double[] values)
        {
            return values.SelectMany(value => BitConverter.GetBytes(value)).ToArray();
        }
       
    }
}