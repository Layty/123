using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ToDoWebApi.Core;

namespace ToDoWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
           services.AddScoped<DlmsDataContext>();
            services.AddDbContext<DlmsDataContext>(option =>
            {
                var str = Configuration.GetConnectionString("DlmsConnectionString");
                option.UseSqlServer(str, sql => sql.MigrationsAssembly("ToDoWebApi"));
            });
            ; services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",new OpenApiInfo(){Description = "Hello API"});
            });
            services.AddTransient<DlmsDataContext>();
            services.AddTransient<DlmsHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var helper = scope.ServiceProvider.GetRequiredService<DlmsHelper>();
                await helper.InitDataBaseAsync();
            }

            app.UseSwagger();
         
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

//            app.UseHttpsRedirection();

            app.UseRouting();

          app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}