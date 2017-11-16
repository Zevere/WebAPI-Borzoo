using System.IO;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    // ToDo: might not be required
    public static class DatabaseInitializer
    {
        public static string ConnectionString;

        public static void InitDatabase(string creationScriptFile) =>
            InitDatabase(ConnectionString, creationScriptFile);

        public static void InitDatabase(string connectionString, string creationScriptFile)
        {
            string sql = File.ReadAllText(creationScriptFile);

            using (var conn = new SqliteConnection(connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static SqliteConnection ConnectAndCreateDatabase(string connectionString, string creationScriptFile)
        {
            string sql = File.ReadAllText(creationScriptFile);

            var conn = new SqliteConnection(connectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            conn.Open();
            cmd.ExecuteNonQuery();
            return conn;
        }

        public static string GetDbFileConnectionString(string path) =>
            "" + new SqliteConnectionStringBuilder
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadWriteCreate,
            };
    }
}