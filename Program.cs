using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradProj.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GradProj
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webhost = CreateHostBuilder(args).Build();
            //RunMigration(webhost);
            webhost.Run();
        }
        private static void RunMigration(IHost host)
        {
            using(var scope = host.Services.CreateScope())
            {
                var db1 = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var db2 = scope.ServiceProvider.GetRequiredService<SiteContext>();
                db1.Database.Migrate();
                db2.Database.Migrate();
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
