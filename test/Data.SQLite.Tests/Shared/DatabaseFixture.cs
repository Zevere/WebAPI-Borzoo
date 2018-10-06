using System.IO;
using Borzoo.Data.SQLite;

namespace Data.SQLite.Tests.Shared
{
    public class DatabaseFixture
    {
        public string ConnectionString { get; }

        public DatabaseFixture()
        {
            ConnectionString = InitializeDatabase();
        }

        /// <summary>
        /// Initializes the SQLite database file. Recreates the database, if exists, and creates its schema
        /// </summary>
        private static string InitializeDatabase()
        {
            var settings = new Settings();

            if (File.Exists(settings.Database))
            {
                File.Delete(settings.Database);
            }

            string connectionString = DatabaseInitializer.GetDbFileConnectionString(settings.Database);
            DatabaseInitializer.EnsureMigrationsApplied(settings.MigrationsScript, connectionString);
            return connectionString;
        }
    }
}