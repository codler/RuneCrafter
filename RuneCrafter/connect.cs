using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using winForm = System.Windows.Forms;

namespace RuneCrafter
{
    class connect
    {
        /* === MYSQL START === */
        private MySqlConnection conn;
        //private MySqlDataAdapter da;
        //private MySqlCommandBuilder cb;


        private string server = "localhost"; 
        private string userid = "username"; 
        private string password = "password"; 
        private string database = "database"; 
        
        public bool isConnected = false;
        /* === MYSQL END === */

        public connect()
        {
            if (conn != null)
                conn.Close();

            string connStr = String.Format("server={0};user id={1}; password={2}; database={3}; pooling=false",
                server, userid, password, database);

            try
            {
                conn = new MySqlConnection(connStr);
                conn.Open();
                isConnected = true;
            }
            catch (MySqlException ex)
            {
                isConnected = false;
                winForm.MessageBox.Show("Error connecting to the server: " + ex.Message);

            }
        }

        public void close()
        {
            if (conn != null)
                conn.Close();
        }

        public List<string> selectOneField(string query)
        {
            return selectOneField(query, 0);
        }

        public List<string> selectOneField(string query, int field)
        {
            MySqlDataReader reader = null;

            MySqlCommand cmd = new MySqlCommand(query, conn);
 
            List<string> value = new List<string>();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    value.Add( reader.GetString(field));
                }
                
            }
            catch (MySqlException ex)
            {
                winForm.MessageBox.Show("Failed to select query: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return value;
        }

        public Dictionary<string,string> selectTwoField(string query)
        {
            MySqlDataReader reader = null;

            MySqlCommand cmd = new MySqlCommand(query, conn);

            Dictionary<string,string> value = new Dictionary<string,string>();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    value.Add(reader.GetString(0), reader.GetString(1));
                }

            }
            catch (MySqlException ex)
            {
                winForm.MessageBox.Show("Failed to select query: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return value;
        }

        public List<Dictionary<string, string>> selectFields(string query)
        {
            MySqlDataReader reader = null;

            MySqlCommand cmd = new MySqlCommand(query, conn);

            List<Dictionary<string, string>> value = new List<Dictionary<string, string>>();
            
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dictionary<string, string> resultValue = new Dictionary<string, string>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        resultValue.Add(reader.GetName(i), reader.GetString(i));
                    }
                    value.Add(resultValue);
                }

            }
            catch (MySqlException ex)
            {
                winForm.MessageBox.Show("Failed to select query: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return value;
        }

        public void updatequery(string query)
        {
            MySqlDataReader reader = null;

            MySqlCommand cmd = new MySqlCommand(query, conn);

            List<string> value = new List<string>();
            try
            {
                cmd.ExecuteScalar();

            }
            catch (MySqlException ex)
            {
                winForm.MessageBox.Show("Failed to update query: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        public long insertquery(string query)
        {
            MySqlDataReader reader = null;

            MySqlCommand cmd = new MySqlCommand(query, conn);

            List<string> value = new List<string>();
            try
            {
                cmd.ExecuteScalar();
                return cmd.LastInsertedId;

            }
            catch (MySqlException ex)
            {
                winForm.MessageBox.Show("Failed to insert query: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            return 0;
        }
    }
}
