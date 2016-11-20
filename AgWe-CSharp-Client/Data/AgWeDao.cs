using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

using AgWe_CSharp_Client.Data.Readings;

namespace AgWe_CSharp_Client.Data
{
    
    class AgWeDao
    {
        public void InitializeDB()
        {
            if (File.Exists("AgweDB.db3"))
            {
                Console.WriteLine("DB Exists");
            }
            else
            {
                this.CreateSQLiteDB();
                Console.WriteLine("DB Created");
            }
        }
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

        public bool SaveReading(string device, string key, string value)
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

        public List<ReadingModel> getLastDay()
        {
            using (SQLiteConnection con = new SQLiteConnection("data source=AgweDB.db3"))
            {
                string sql = "select * from [AgweData] where Timestamp <= $Timestamp and Timestamp >= $TimeStampMin24";
                using (SQLiteCommand command = new SQLiteCommand(con))
                {
                    var now = DateTime.Now;
                    var yesterday = now.AddDays(-1);
                    con.Open();

                    command.CommandText = sql;
                    command.Parameters.AddWithValue("$Timestamp", now);
                    command.Parameters.AddWithValue("$TimeStampMin24", yesterday);                    

                    SQLiteDataReader reader = command.ExecuteReader();
                    List<ReadingModel> readings = new List<ReadingModel>();
                    if (reader.HasRows)
                    {
                        
                        while (reader.Read())
                            //Console.WriteLine("Device: " + reader["device"] + "\tTimestamp: " + reader["Timestamp"] + "\tKey: " + reader["Key"] + "\tValue: " + reader["Value"]);
                            readings.Add(new ReadingModel { DateTime = System.Convert.ToDateTime(reader["Timestamp"]), Value = Convert.ToDouble(reader["Value"]), kind = Convert.ToString(reader["Key"])});
                    }
                    con.Close();
                    return readings;
                }
            }
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
                        Console.WriteLine("Device: " + reader["device"] + "\tTimestamp: " + reader["Timestamp"] + "\tKey: " + reader["Key"] + "\tValue: " + reader["Value"]);

                    con.Close();
                }
            }
        }
    }
}
