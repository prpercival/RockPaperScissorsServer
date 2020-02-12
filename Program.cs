using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RockPaperScissorsServer
{
    public class Program
    {
        private static string _urls { get; set; }

        public static void Main(string[] args)
        {
            if (Debugger.IsAttached)
                _urls = "http://localhost:45145;https://localhost:45146";
            else
                _urls = "http://192.168.1.5:45145;https://192.168.1.5:45146";

            Console.WriteLine(_urls);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseKestrel()
                    .UseUrls(_urls);
                });
    }
}
