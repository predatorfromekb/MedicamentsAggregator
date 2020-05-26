using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MedicamentsAggregator.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var port = Environment.GetEnvironmentVariable("PORT");

            var builderWithStartup = WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            return port == null
                ? builderWithStartup
                : builderWithStartup.UseUrls("http://*:" + port);
        }
    }
}