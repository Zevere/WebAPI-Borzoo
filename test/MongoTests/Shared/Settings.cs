using System.IO;
using Microsoft.Extensions.Configuration;

namespace MongoTests.Shared
{
    public class Settings
    {
        public string ConnectionString { get; }

        public Settings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .AddJsonEnvVar("BORZOO_TEST_SETTINGS", optional: true)
                .Build();

            ConnectionString = configuration[nameof(ConnectionString)];
        }
    }
}