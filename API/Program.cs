using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((builderContext, confBuilder) =>
            {
                var env = builderContext.HostingEnvironment;
                confBuilder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                confBuilder.AddEnvironmentVariables();
            })
            .UseStartup<Startup>();

        //Host.CreateDefaultBuilder(args)
        //    .ConfigureWebHostDefaults((builderContext, webBuilder) =>
        //    {
        //        var env = builderContext
        //        webBuilder.UseStartup<Startup>();
        //    });
    }
}
