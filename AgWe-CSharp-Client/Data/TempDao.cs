using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace AgWe_CSharp_Client.Data
{
    class TempDao
    {
        public bool CreateSQLiteDB(bool fresh = false)
        {
            if (fresh) DropTable();

            string createTableQuery = @"CREATE TABLE IF NOT EXISTS [AgweData] (
                        [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [Device] NVARCHAR(2048)  NULL,
                        [Timestamp] DATETIME  NULL,
                        [Key] NVARCHAR(256) NULL,
                        [Value] NVARCHAR(256) NULL
                        )";

            using (SQLiteConnection con = new SQLiteConnection("data source=AgweDB.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    con.Open();

                    com.CommandText = createTableQuery;
                    com.ExecuteNonQuery();

                    con.Close();
                }
            }

            return File.Exists("AgweDB.db3");
        }

        private void DropTable()
        {
            string drop = @"DROP TABLE IF EXISTS [AgweData]";
            using (SQLiteConnection con = new SQLiteConnection("data source=AgweDB.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    con.Open();

                    com.CommandText = drop;
                    com.ExecuteNonQuery();

                    con.Close();
                }
            }
        }

        public bool Insert(string device, string key, string value)
        {
            DateTime dt = DateTime.Now;
            bool inserted = false;

            string insertCommand = @"INSERT INTO [AgweData] ([Timestamp], [Device], [Key], [Value]) 
                        VALUES ($timestamp, $device, $key, $value)";

            using (SQLiteConnection con = new SQLiteConnection("data source=AgweDB.db3"))
            {
                using (SQLiteCommand comm = new SQLiteCommand(con))
                {
                    con.Open();

                    comm.CommandText = insertCommand;
                    comm.Parameters.AddWithValue("$timestamp", dt);
                    comm.Parameters.AddWithValue("$device", device);
                    comm.Parameters.AddWithValue("$key", key);
                    comm.Parameters.AddWithValue("$value", value);

                    inserted = Convert.ToBoolean(comm.ExecuteNonQuery());

                    con.Close();
                }
            }

            return inserted;
        }

        public void Read()
        {
            using (SQLiteConnection con = new SQLiteConnection("data source=AgweDB.db3"))
            {
                string sql = "select * from [AgweData]";
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    command.CommandText = sql;

                    con.Open();

                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                        Console.WriteLine("Device: " + reader["device"] + "\tTimestamp: " + reader["Timestamp"] + "\tKey: " + reader["Key"] + "\tKey: " + reader["Value"]);

                    con.Close();
                }
            }
        }
    }
}
