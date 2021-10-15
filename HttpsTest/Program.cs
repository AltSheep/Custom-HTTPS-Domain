using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpsTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => 
                {
                    HostConfig.CERTIFICATION_PATH = context.Configuration["CertPath"];
                    HostConfig.CERTIFICATION_PASSWORD = context.Configuration["CertPassword"];
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var host = Dns.GetHostEntry("weather.io");


                    webBuilder.ConfigureKestrel(opt => 
                    {
                        opt.Listen(host.AddressList[0], 5000);
                        //opt.ListenAnyIP(5000);
                        opt.Listen(host.AddressList[0], 5001, listOpt => 
                        {
                            listOpt.UseHttps(HostConfig.CERTIFICATION_PATH, HostConfig.CERTIFICATION_PASSWORD);
                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class HostConfig
    {
        public static string CERTIFICATION_PATH { get; set; }
        public static string CERTIFICATION_PASSWORD { get; set; }
    }
}
