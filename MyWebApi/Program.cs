using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyWebApi.Data;

namespace MyWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build(); 
            using (var scope1 = host.Services.CreateScope())
            {
                try
                {
                    var userLoginDbContext = scope1.ServiceProvider.GetService<UserLoginDbContext>();
                    userLoginDbContext.Database.EnsureCreated();
                    userLoginDbContext.Database.Migrate();
                }
                catch (Exception e)
                {
                    var logger = scope1.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "UserLogin Database Migration Error!");
                }
            }
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var cosemObjectDbContext = scope.ServiceProvider.GetService<CosemObjectDbContext>();
                    cosemObjectDbContext.Database.EnsureCreated();
                    cosemObjectDbContext.Database.Migrate();
                }
                catch (Exception e)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "Database Migration Error!");
                }
            }

          

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}