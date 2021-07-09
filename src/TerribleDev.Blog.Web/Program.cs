using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TerribleDev.Blog.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            var builder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureKestrel(a =>
                {
                    a.AddServerHeader = false;
                });
            var port = Environment.GetEnvironmentVariable("PORT");
            if(!String.IsNullOrWhiteSpace(port)) {
                builder.UseUrls("http://*:" + port);
            }
            return builder;
        }
    }
}
