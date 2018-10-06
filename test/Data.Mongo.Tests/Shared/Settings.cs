using System.IO;
using Microsoft.Extensions.Configuration;

namespace Data.Mongo.Tests.Shared
{
    public class Settings
    {
        public string Connection { get; }

        public Settings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .AddJsonEnvVar("BORZOO_TEST_SETTINGS", optional: true)
                .Build();

            Connection = configuration[nameof(Connection)];
        }
    }
}