using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlackDragonAIAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var portEnv = Environment.GetEnvironmentVariable("PORT");
                    var port = !string.IsNullOrEmpty(portEnv) ? int.Parse(portEnv) : 80;

                    webBuilder.UseStartup<Startup>().UseUrls("http://*:" + port);
                });
    }
}
