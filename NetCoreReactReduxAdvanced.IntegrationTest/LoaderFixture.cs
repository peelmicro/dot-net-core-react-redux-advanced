using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;

namespace NetCoreReactReduxAdvanced.IntegrationTest
{
    public class LoaderFixture: IDisposable
    {
        IWebHost _webHost = null;

        public string Url {get; set;} = "https://localhost:5001/";
        public Browser Browser { get; set; }
        public TestServer TestServer { get; private set; }
        public HttpClient HttpClient { get; private set; }


        public LoaderFixture()
        {
            SetupAsync().GetAwaiter().GetResult();
        }
        public void Dispose()
        {
            Task.Run(DisposeAsync).Wait();
        }
        public async Task DisposeAsync()
        {
            TestServer.Dispose();
            await Browser.CloseAsync();
            await _webHost.StopAsync();
        }
        private async Task SetupAsync()
        {
            await StartWebServerAsync();
        }

        private async Task StartWebServerAsync()
        {
            var mainApp = Assembly.GetExecutingAssembly().FullName.Split(',').First().Replace(".IntegrationTest","");
            var mainPath = Path.GetFullPath($"../../../../{mainApp}");

            _webHost = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseEnvironment("Production")
                .UseUrls(Url)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(mainPath);
                    config.AddJsonFile("appsettings.json", optional: true);
                })
                .UseContentRoot(mainPath)
                .Build();
            await _webHost.StartAsync();

            TestServer = new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseContentRoot(mainPath)
                    .UseEnvironment("Development")
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.SetBasePath(mainPath);
                        config.AddJsonFile("appsettings.json", optional: true);
                    })
                );
            HttpClient = TestServer.CreateClient();
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions{Headless = true});
        }
    }
}
