using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo.Web.Tests.Integ
{
    public class TestHostFixture<TStartup> : IDisposable
    {
        public HttpClient Client { get; }

        private readonly TestServer _server;

        private readonly string _testDbPath;

        public TestHostFixture()
            : this(Path.Combine("src"))
        {
        }

        private TestHostFixture(string relativeTargetProjectParentDir)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            string contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServices)
                .UseEnvironment("Development")
                .UseConfiguration(BuildConfiguration())
                .UseStartup(typeof(TStartup));

            _server = new TestServer(builder);

            _testDbPath = _server.Host.Services.GetRequiredService<IConfiguration>()["SQLite_Db_Path"];

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json")
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
                directoryInfo = directoryInfo.Parent;

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
            File.Delete(_testDbPath);
            Client.Dispose();
            _server.Dispose();
        }
    }
}