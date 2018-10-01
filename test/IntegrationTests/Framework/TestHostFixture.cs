using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace IntegrationTests.Framework
{
    public class TestHostFixture<TStartup> : IDisposable
    {
        public HttpClient Client { get; }

        private readonly TestServer _server;

        private readonly string _webAppContentRoot;

        private readonly string _testAppContentRoot;

        private string _testSQLiteDb;

        public TestHostFixture()
        {
            _testAppContentRoot = Path.GetFullPath(Path.Combine(
                Assembly.GetExecutingAssembly().Location, "..", "..", "..", "..")
            );
            _webAppContentRoot = Path.GetFullPath(Path.Combine(
                Assembly.GetExecutingAssembly().Location, "..", "..", "..", "..", "..", "..", "src",
                "Borzoo.Web"
            ));

            var configuration = BuildConfiguration();
            var builder = new WebHostBuilder()
                .UseContentRoot(_webAppContentRoot)
                .ConfigureServices(InitializeServices)
                .UseEnvironment("Development")
                .UseConfiguration(configuration)
                .UseStartup(typeof(TStartup));

            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public async Task<HttpResponseMessage> SendGraphQLRequest(
            string query,
            object variables = default,
            string operationName = default,
            CancellationToken cancellationToken = default
        )
        {
            string payload = JsonConvert.SerializeObject(new
            {
                query,
                variables,
                operationName
            }, new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Ignore});

            var resp = await Client.PostAsync("/zv/graphql", new StringContent(
                payload, Encoding.UTF8, "application/json"
            ), cancellationToken);
            return resp;
        }

        private IConfigurationRoot BuildConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(_webAppContentRoot)
                .AddJsonFile("appsettings.json")
                .AddJsonFile(new PhysicalFileProvider(_testAppContentRoot), "appsettings.Development.json", true, false)
                .AddJsonEnvVar("BORZOO_TEST_SETTINGS", true)
                .Build();

            if (configuration["data:use"] == "sqlite")
            {
                _testSQLiteDb = Path.GetTempFileName();
                configuration["data:sqlite:db"] = _testSQLiteDb;

                string migrationsPath = configuration["data:sqlite:migrations"];
                if (!Path.IsPathFullyQualified(migrationsPath))
                {
                    configuration["data:sqlite:migrations"] =
                        Path.GetFullPath(Path.Combine(_webAppContentRoot, migrationsPath));
                }
            }

            return configuration;
        }

        private static void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;

            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());

            services.AddSingleton(manager);
        }

        public void Dispose()
        {
            if (_testSQLiteDb != null)
            {
                try
                {
                    File.Delete(_testSQLiteDb);
                }
                catch (IOException)
                {
                    Console.WriteLine($@"Unable to delete database file ""{_testSQLiteDb}""");
                }
            }

            Client.Dispose();
            _server.Dispose();
        }
    }
}