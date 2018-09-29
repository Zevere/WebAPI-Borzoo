using System.IO;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    // ToDo: might not be required
    public static class DatabaseInitializer
    {
        public static string ConnectionString;

        public static bool EnsureMigrationsApplied(string migrationsScriptFile)
        {
            bool databaseMigrated;
            string sql = File.ReadAllText(migrationsScriptFile);

            using (var conn = new SqliteConnection(ConnectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(tbl_name) FROM sqlite_master";

                conn.Open();
                bool tablesExist = int.Parse(cmd.ExecuteScalar().ToString()) > 0;

                if (!tablesExist)
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                databaseMigrated = !tablesExist;
            }

            return databaseMigrated;
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
            new SqliteConnectionStringBuilder
            {
                DataSource = Path.GetFullPath(path),
                Mode = SqliteOpenMode.ReadWriteCreate,
            }.ToString();
    }
}