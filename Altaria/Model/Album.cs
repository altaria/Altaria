using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Altaria.Model
{
    public class Album
    {
        public int albumId { get; set; }
        public string albumName { get; set; }

        public Album(int albumId)
        {
            this.albumId = albumId;
        }

        public Album(string albumName)
        {
            this.albumName = albumName;
        }

        public Album(int albumId, string albumName)
        {
            this.albumId = albumId;
            this.albumName = albumName;
        }

        public bool InsertAlbum()
        {
            DBConnect connection = new DBConnect();

            string query = "INSERT INTO Album(albumName) VALUES(@albumName);";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@albumName", this.albumName);

                cmd.ExecuteNonQuery();

                connection.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateAlbum()
        {
            DBConnect connection = new DBConnect();

            string query = "UPDATE Album SET albumName=@albumName WHERE albumId=@id;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@albumName", this.albumName);
                cmd.Parameters.AddWithValue("@id", this.albumId);

                cmd.ExecuteNonQuery();

                connection.CloseConnection();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int retrieveAlbumId()
        {
            DBConnect connection = new DBConnect();
            
            string query = "SELECT albumId FROM Album WHERE albumName LIKE @albumName;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, connection.OpenConnection());
                cmd.CommandText = query;
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@albumName", this.albumName);

                albumId = int.Parse(cmd.ExecuteScalar() + "");

                cmd.ExecuteNonQuery();

                connection.CloseConnection();

                return albumId;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}