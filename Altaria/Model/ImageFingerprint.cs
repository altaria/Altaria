using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Collections;

namespace Altaria.Model
{
    public class ImageFingerprint
    {
        private int imageId;
        private string imageTitle;
        private string imageAuthor;
        private string digestId;
        private byte[] digestCoeffs;
        private ulong hash;

        public void setDigestId(string digestId)
        {
            this.digestId = digestId;
        }

        public string getDigestId()
        {
            return digestId;
        }

        public void setDigestCoeffs(byte[] digestCoeffs)
        {
            this.digestCoeffs = digestCoeffs;
        }

        public byte[] getDigestCoeffs()
        {
            return digestCoeffs;
        }

        public void setImageTitle(string imageTitle)
        {
            this.imageTitle = imageTitle;
        }

        public string getImageTitle()
        {
            return imageTitle;
        }

        public void setImageAuthor(string imageAuthor)
        {
            this.imageAuthor = imageAuthor;
        }

        public string getImageAuthor()
        {
            return imageAuthor;
        }

        public ulong getHash()
        {
            return hash;
        }

        public void setHash(ulong hash)
        {
            this.hash = hash;
        }

        public ImageFingerprint()
        {
        }

        public ImageFingerprint(string imageTitle, string imageAuthor, string digestId, byte[] digestCoeffs, ulong hash)
        {
            this.imageTitle = imageTitle;
            this.imageAuthor = imageAuthor;
            this.digestId = digestId;
            this.digestCoeffs = digestCoeffs;
            this.hash = hash;
        }

        public bool insertNewImage()
        {
            DBConnect connection = new DBConnect();

            string query = "INSERT INTO image(imageTitle, imageAuthor, digestId, digestHash) VALUES(@imageTitle, @imageAuthor, @digestId, @hash)";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@imageTitle", this.imageTitle);
                cmd.Parameters.AddWithValue("@imageAuthor", this.imageAuthor);
                if (!string.IsNullOrEmpty(digestId))
                    cmd.Parameters.AddWithValue("@digestId", this.digestId);
                else
                    cmd.Parameters.AddWithValue("@digestId", "n");
                cmd.Parameters.AddWithValue("@hash", this.hash);

                cmd.ExecuteNonQuery();

                query = "SELECT LAST_INSERT_ID();";
                cmd.CommandText = query;
                cmd.Prepare();

                int imageId = Convert.ToInt32(cmd.ExecuteScalar());

                for (int i = 0; i < digestCoeffs.Count(); i++)
                {
                    query = "INSERT INTO imageCoeffs(imageId, value) VALUES(@imageId, @value);";
                    cmd.CommandText = query;
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@imageId", imageId);
                    cmd.Parameters.AddWithValue("@value", digestCoeffs[i]);

                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                }

                connection.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ArrayList retrieveAllHashes()
        {
            DBConnect connection = new DBConnect();
            ArrayList imageFingerprint = new ArrayList();
            string query = "SELECT i.imageId, i.imageTitle, i.imageAuthor, i.digestId, i.digestHash, ic.value FROM image i INNER JOIN imageCoeffs ic ON i.imageId = ic.imageId;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();

                MySqlDataReader dataReader = cmd.ExecuteReader();
                int i = 0;
                byte[] digestCoeffs = new byte[40];

                while (dataReader.Read())
                {
                    int imageId = (int)dataReader[0];
                    string imageTitle = (string)dataReader[1];
                    string imageAuthor = (string)dataReader[2];
                    string digestId = (string)dataReader[3];
                    ulong hash = (ulong)dataReader[4];

                    if (digestId.Equals("n"))
                        digestId = null;

                    int temp = (int)dataReader[5];
                    digestCoeffs[i] = (byte)temp;
                    i++;

                    if (i == 40)
                    {
                        ImageFingerprint image = new ImageFingerprint(imageTitle, imageAuthor, digestId, digestCoeffs, hash);
                        imageFingerprint.Add(image);
                        i = 0;
                        digestCoeffs = new byte[40];
                    }
                }

                return imageFingerprint;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}