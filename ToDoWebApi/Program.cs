using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDoWebApi.Core;

namespace ToDoWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
//            using (var scope = host.Services.CreateScope())
//            {
//                try
//                {
//                    var dbContext = scope.ServiceProvider.GetService<DlmsDataContext>();
//                    dbContext.Database.EnsureDeleted();
//                    dbContext.Database.Migrate();
//                }
//                catch (Exception e)
//                {
//                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//                    logger.LogError(e,"Migration Error!");
//                }
//            
//                
//            }


            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}