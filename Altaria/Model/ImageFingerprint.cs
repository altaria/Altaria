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
        private IntPtr digestCoeffs;
        private int digestSize;
        private ulong hash;

        public void setDigestId(string digestId)
        {
            this.digestId = digestId;
        }

        public string getDigestId()
        {
            return digestId;
        }

        public void setDigestCoeffs(IntPtr digestCoeffs)
        {
            this.digestCoeffs = digestCoeffs;
        }

        public IntPtr getDigestCoeffs()
        {
            return digestCoeffs;
        }

        public void setDigestSize(int digestSize)
        {
            this.digestSize = digestSize;
        }

        public int getDigestSize()
        {
            return digestSize;
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

        public ImageFingerprint(string imageTitle, string imageAuthor, string digestId, IntPtr digestCoeffs, int digestSize, ulong hash)
        {
            this.imageTitle = imageTitle;
            this.imageAuthor = imageAuthor;
            this.digestId = digestId;
            this.digestCoeffs = digestCoeffs;
            this.digestSize = digestSize;
            this.hash = hash;
        }

        public bool insertNewImage()
        {
            DBConnect connection = new DBConnect();

            string query = "INSERT INTO image(imageTitle, imageAuthor, digestId, digestCoeffs, digestSize, digestHash) VALUES(@imageTitle, @imageAuthor, @digestId, @digestCoeffs, @digestSize, @hash)";

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
                cmd.Parameters.AddWithValue("@digestCoeffs", this.digestCoeffs);
                cmd.Parameters.AddWithValue("@digestSize", this.digestSize);
                cmd.Parameters.AddWithValue("@hash", this.hash);

                cmd.ExecuteNonQuery();

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

            string query = "SELECT imageTitle, imageAuthor, digestId, digestCoeffs, digestSize, digestHash FROM image";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();

                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    string imageTitle = (string)dataReader[0];
                    string imageAuthor = (string)dataReader[1];
                    string digestId = (string)dataReader[2];
                    int digestCoeffs = (int)dataReader[3];
                    int digestSize = (int)dataReader[4];
                    ulong hash = (ulong)dataReader[5];

                    if (digestId.Equals("n"))
                        digestId = null;

                    ImageFingerprint image = new ImageFingerprint(imageTitle, imageAuthor, digestId, new IntPtr(digestCoeffs), digestSize, hash);
                    imageFingerprint.Add(image);
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