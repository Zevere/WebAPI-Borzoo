using System.IO;
using Microsoft.Extensions.Configuration;

namespace Data.SQLite.Tests.Shared
{
    public class Settings
    {
        public string Database { get; }

        public string MigrationsScript { get; }

        public Settings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .AddJsonEnvVar("BORZOO_TEST_SETTINGS", optional: true)
                .Build();

            Database = configuration[nameof(Database)];
            MigrationsScript = configuration[nameof(MigrationsScript)];
        }
    }
}