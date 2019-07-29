using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Uptime.CredentialManager.Web.Models;
using Microsoft.Extensions.Hosting;

namespace Uptime.CredentialManager.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //1. Get the IWebHost which will host this application.
            var host = CreateHostBuilder(args).Build();

            //2. Find the service layer within our scope.
            using (var scope = host.Services.CreateScope())
            { //3. Get the instance of UptimeCredentialManagerWebContext in our services layer
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<UptimeCredentialManagerWebContext>();
                               
                context.Database.EnsureCreated();
            }

            //Continue to run the application
            host.Run();

        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
