using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Collections;

namespace Altaria_DRM.Model
{
    public class SongRetrieval
    {
        private int songId;
        private int artistId;
        private int albumId;
        private string songTitle;
        private int songBitrate;
        private string songHash;
        private int songOffset;
        private ArrayList songHashes;
        private ArrayList hashId;
        private ArrayList songIdList;
        private int songHashId;
        private string artistName;
        private string albumName;

        public int getSongId()
        {
            return songId;
        }

        public void setSongId(int songId)
        {
            this.songId = songId;
        }

        public string getSongHash()
        {
            return songHash;
        }

        public void setSongHash(string songHash)
        {
            this.songHash = songHash;
        }

        public void setSongOffset(int songOffset)
        {
            this.songOffset = songOffset;
        }

        public int getSongOffset()
        {
            return songOffset;
        }

        public void setSongHashes(ArrayList songHashes)
        {
            this.songHashes = songHashes;
        }

        public ArrayList getSongHashes()
        {
            return songHashes;
        }

        public void setSongIdList(ArrayList songIdList)
        {
            this.songIdList = songIdList;
        }

        public ArrayList getSongIdList()
        {
            return songIdList;
        }

        public void setHashId(ArrayList hashId)
        {
            this.hashId = hashId;
        }

        public ArrayList getHashId()
        {
            return hashId;
        }

        public string getSongTitle()
        {
            return songTitle;
        }

        public void setSongTitle(string songTitle)
        {
            this.songTitle = songTitle;
        }

        public string getAlbumName()
        {
            return albumName;
        }

        public void setAlbumName(string albumName)
        {
            this.albumName= albumName;
        }

        public string getArtistName()
        {
            return artistName;
        }

        public void setArtistName(string artistName)
        {
            this.artistName = artistName;
        }

        public SongRetrieval()
        {
        }

        public SongRetrieval(int songId, bool song)
        {
            if (song)
                this.songId = songId;
            else
                this.songHashId = songId;
        }

        public SongRetrieval(string songHash)
        {
            this.songHash = songHash;
        }

        public SongRetrieval(int songId, string songHash, int songOffset)
        {
            this.songId = songId;
            this.songHash = songHash;
            this.songOffset = songOffset;
        }

        public SongRetrieval(int artistId, int albumId, string songTitle, int songBitrate)
        {
            this.artistId = artistId;
            this.albumId = albumId;
            this.songTitle = songTitle;
            this.songBitrate = songBitrate;
        }

        public SongRetrieval(string songTitle, string artistName, string albumName)
        {
            this.songTitle = songTitle;
            this.artistName = artistName;
            this.albumName = albumName;
        }

        public bool InsertSong()
        {
            DBConnect connection = new DBConnect();

            string query = "INSERT INTO song(artistId, albumId, songTitle, songBitrate) VALUES(@artistId, @albumId, @songTitle, @songBitrate);";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@artistId", this.artistId);
                cmd.Parameters.AddWithValue("@albumId", this.albumId);
                cmd.Parameters.AddWithValue("@songTitle", this.songTitle);
                cmd.Parameters.AddWithValue("@songBitrate", this.songBitrate);

                cmd.ExecuteNonQuery();

                connection.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int retrieveLatestSongId()
        {
            DBConnect connection = new DBConnect();

            string query = "SELECT max(songId) FROM song;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();

                songId = int.Parse(cmd.ExecuteScalar() + "");

                connection.CloseConnection();

                return songId;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public bool InsertSongHash()
        {
            DBConnect connection = new DBConnect();

            string query = "INSERT INTO songhash(songId, songHash, songOffset) VALUES(@songId, @songHash, @songOffset);";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@songId", this.songId);
                cmd.Parameters.AddWithValue("@songHash", this.songHash);
                cmd.Parameters.AddWithValue("@songOffset", this.songOffset);

                cmd.ExecuteNonQuery();

                connection.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string retrieveSongFromHashId()
        {
            DBConnect connection = new DBConnect();

            string query = "SELECT songTitle FROM song s INNER JOIN songhash sh ON sh.songId = s.songId WHERE sh.hashId = @hashId;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@hashId", this.songHashId);

                string songTitle = (string)cmd.ExecuteScalar();

                connection.CloseConnection();

                return songTitle;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*public void retrieveAllHash()
        {
            DBConnect connection = new DBConnect();
            ArrayList hashValues = new ArrayList();

            string query = "SELECT hashId, songHash FROM songhash;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();

                MySqlDataReader dataReader = cmd.ExecuteReader();

                hashId = new ArrayList();
                hashValues = new ArrayList();

                while (dataReader.Read())
                {
                    hashId.Add(dataReader[0]);
                    hashValues.Add(dataReader[1]);
                }

                this.setHashId(hashId);
                this.setSongHashes(hashValues);

                connection.CloseConnection();
            }
            catch (Exception ex)
            {
                
            }
        }*/

        public void retrieveAllHash()
        {
            DBConnect connection = new DBConnect();
            ArrayList hashValues = new ArrayList();

            string query = "SELECT songId, songHash FROM songhash;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();

                MySqlDataReader dataReader = cmd.ExecuteReader();

                songIdList = new ArrayList();
                hashValues = new ArrayList();

                while (dataReader.Read())
                {
                    songIdList.Add(dataReader[0]);
                    hashValues.Add(dataReader[1]);
                }

                this.setSongIdList(songIdList);
                this.setSongHashes(hashValues);

                connection.CloseConnection();
            }
            catch (Exception ex)
            {

            }
        }

        public SongRetrieval retrieveSongFromSongId()
        {
            SongRetrieval sr = null;
            DBConnect connection = new DBConnect();

            string query = "SELECT s.songTitle, ar.artistName, al.albumName FROM song s INNER JOIN artist ar ON s.artistId = ar.artistId INNER JOIN album al ON s.albumId = al.albumId WHERE s.songId = @songId;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@songId", this.songId);

                //string songTitle = (string)cmd.ExecuteScalar();
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while(dataReader.Read())
                    sr = new SongRetrieval((string)dataReader[0], (string)dataReader[1], (string)dataReader[2]);

                connection.CloseConnection();

                return sr;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}