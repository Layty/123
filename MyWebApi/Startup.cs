using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MyWebApi.Data;
using MyWebApi.Services;

namespace MyWebApi
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "CosemWebApi", Version = "v1"});
            });
            services.AddScoped<ICosemRepository, CosemRepository>();
            services.AddDbContext<CosemObjectDbContext>(option => { option.UseSqlite("Data Source=CosemObjects.db"); });
            services.AddScoped<IUserLoginRepository, UserLoginRepository>();
            services.AddDbContext<UserLoginDbContext>(option => { option.UseSqlite("Data Source=UserLogin.db"); });
            services.AddScoped<IMeterRepository, MeterRepository>();
            services.AddDbContext<MeterDbContext>(option => { option.UseSqlite("Data Source = Meter.db"); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CosemWebApi v1"));


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}