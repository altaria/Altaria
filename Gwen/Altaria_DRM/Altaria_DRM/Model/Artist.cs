using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Altaria_DRM.Model
{
    public class Artist
    {
        public int artistId { get; set; }
        public string artistName { get; set; }

        public Artist(int artistId)
        {
            this.artistId = artistId;
        }

        public Artist(string artistName)
        {
            this.artistName = artistName;
        }

        public Artist(int artistId, string artistName)
        {
            this.artistId = artistId;
            this.artistName = artistName;
        }

        public bool InsertArtist()
        {
            DBConnect connection = new DBConnect();

            string query = "INSERT INTO Artist(artistName) VALUES(@artistName);";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@artistName", this.artistName);

                cmd.ExecuteNonQuery();

                connection.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateArtist()
        {
            DBConnect connection = new DBConnect();

            string query = "UPDATE Artist SET artistName=@artistName WHERE artistId=@id;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@artistName", this.artistName);
                cmd.Parameters.AddWithValue("@id", this.artistId);

                cmd.ExecuteNonQuery();

                connection.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int retrieveArtistId()
        {
            DBConnect connection = new DBConnect();
            
            string query = "SELECT artistId FROM Artist WHERE artistName LIKE @artistName;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@artistName", this.artistName);

                artistId = int.Parse(cmd.ExecuteScalar() + "");

                cmd.ExecuteNonQuery();

                connection.CloseConnection();

                return artistId;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}