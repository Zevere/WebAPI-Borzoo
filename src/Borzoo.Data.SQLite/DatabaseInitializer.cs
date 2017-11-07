using System.IO;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    public static class DatabaseInitializer
    {
        public static string ConnectionString;

        public static void InitDatabase(string creationScriptFile)
        {
            string sql = File.ReadAllText(creationScriptFile);

            using (var conn = CreateConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        internal static SqliteConnection CreateConnection() => new SqliteConnection(
            "" + new SqliteConnectionStringBuilder {DataSource = ConnectionString}
        );
    }
}