using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NetCoreReactReduxAdvanced
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
//        public static void Main(string[] args)
//        {
//            BuildWebHost(args).Run();
//        }
//
//        public static IWebHost BuildWebHost(string[] args) {
//            return WebHost.CreateDefaultBuilder(args)
//                .UseStartup<Startup>()
//                .Build();
//        }
    }
}
