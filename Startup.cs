using AspNetCoreRateLimit;
using HotelListingAPI.Data.Configurations;
using HotelListingAPI.Data.Interfaces;
using HotelListingAPI.Data.Repos;
using HotelListingAPI.Data.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog.Core;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI
{
    public class Startup
    {
        private static readonly string AllowOrigins = "AllowAll";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<HotelDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("HotelAPI"))
            );

            services.AddAuthentication();
            services.ConfigureIdentity();
            services.ConfigureJwt(Configuration);
            services.ConfigureVersioning();
            services.ConfigureHttpCacheHeaders();
            services.ConfigureRateLimiting();
            services.AddHttpContextAccessor();

            services.AddMemoryCache();

            //services.AddResponseCaching();

            services.AddCors(cs => {
                cs.AddPolicy(AllowOrigins,
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddAutoMapper(typeof(MapperInitializer));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { Title = "HotelListingAPI", Version = "v1" });
            });

            services.AddControllers(config => {
                config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
                {
                    Duration = 120
                });
            }).AddNewtonsoftJson(
                op => op.SerializerSettings
                        .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthManager, AuthManager>();

        } 

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                //c.DefaultModelRendering(ModelRendering.Example);
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListingAPI");
                //c.RoutePrefix = string.Empty;
                string swaggerJsonPath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonPath}/swagger/v1/swagger.json", "HotelListingAPI");
            });

            app.ConfigureExceptionHandler();
            app.UseHttpsRedirection();

            app.UseCors("AllowOrigins");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseIpRateLimiting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");


                endpoints.MapControllers();
            });
        }
    }
}
