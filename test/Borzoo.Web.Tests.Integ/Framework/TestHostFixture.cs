using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo.Web.Tests.Integ.Framework
{
    public class TestHostFixture<TStartup> : IDisposable
    {
        public HttpClient Client { get; }

        private readonly TestServer _server;

        private string _testDbPath;

        private readonly string _contentRoot;

        public TestHostFixture()
            : this(Path.Combine("src"))
        {
        }

        private TestHostFixture(string relativeTargetProjectParentDir)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            _contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var builder = new WebHostBuilder()
                .UseContentRoot(_contentRoot)
                .ConfigureServices(InitializeServices)
                .UseEnvironment("Development")
                .UseConfiguration(BuildConfiguration())
                .UseStartup(typeof(TStartup));

            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        private IConfigurationRoot BuildConfiguration()
        {
            _testDbPath = Path.GetTempFileName();
            string migrationsFile = Path.GetFullPath(Path.Combine(
                _contentRoot, "..", "Borzoo.Data.SQLite", "scripts", "migrations.sql"
            ));
            KeyValuePair<string, string>[] settings =
            {
                new KeyValuePair<string, string>("SQLite_Db_Path", _testDbPath),
                new KeyValuePair<string, string>("SQLite_Migrations_Script", migrationsFile),
            };

            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddInMemoryCollection(settings)
                .Build();
        }

        private static void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;

            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());

            services.AddSingleton(manager);
        }

        private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            var projectName = startupAssembly.GetName().Name;
            var applicationBasePath = AppContext.BaseDirectory;

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                directoryInfo = directoryInfo.Parent ?? throw new DirectoryNotFoundException();

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName,
                        $"{projectName}.csproj"));
                    if (projectFileInfo.Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            } while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

        public void Dispose()
        {
            try
            {
                File.Delete(_testDbPath);
            }
            catch (IOException)
            {
                Console.WriteLine($@"Unable to delete database file ""{_testDbPath}""");
            }

            Client.Dispose();
            _server.Dispose();
        }
    }
}