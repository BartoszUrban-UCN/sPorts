using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebApplication.Data;

namespace WebApplication
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                });

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //Application startup - connect to the DB
            await ConnectDb(host);

            host.Run();
        }

        public static async Task ConnectDb(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<SportsContext>();
                    await DbInitializer.InitializeDb(services, context);
                    await RoleInitializer.SeedRoles(services, context);
                    //await DbInitializer.EnsureMarinaOwner(services, "123456", "marinaowner@gmail.com");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}
