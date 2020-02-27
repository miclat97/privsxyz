using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PrivsXYZ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).UseUrls("http://localhost:7000/").Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureKestrel((context, serverOptions) =>
                {
                    serverOptions.Limits.KeepAliveTimeout =
                        TimeSpan.FromMinutes(10);
                    serverOptions.Limits.RequestHeadersTimeout =
                        TimeSpan.FromMinutes(10);
                });
    }
}
