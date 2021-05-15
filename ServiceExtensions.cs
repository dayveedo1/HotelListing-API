using AspNetCoreRateLimit;
using HotelListingAPI.Data;
using HotelListingAPI.Data.Exceptions;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListingAPI
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUser>(x => x.User.RequireUniqueEmail = true);
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddEntityFrameworkStores<HotelDbContext>().AddDefaultTokenProviders();
        }

        public static void ConfigureJwt (this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            //set the key value which we saved as a local variable on localhost
            var key = Environment.GetEnvironmentVariable("SKEY");

            services.AddAuthentication(opt =>
            {
                //adding Authentication to the application, which i'm setting the default to JWT
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //check for whatever request the application recieves & verify it is valid
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    //parameters it should use to validate a token it recieves
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //validate the issuer which we set in the appsetting.json
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateActor = false,
                        //validate the Lifesan duration of the token
                        ValidateLifetime = true,
                        //validate the signing key which we created
                        ValidateIssuerSigningKey = true,
                        //the valid issuer is Issuer gotten from the appsetting.json
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        //Encrypting the issuer signing key
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                };
                });
        }

        public static void ConfigureExceptionHandler (this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if(contextFeature != null)
                    {
                        Log.Error($"Something went wrong in {contextFeature.Error}");

                        await context.Response.WriteAsync(new Error
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error, Please Try Again"
                        }.ToString());
                    }
                });
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(
                (expirationOpt) =>
                {
                    expirationOpt.MaxAge = 60;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },
                (validatorOpt) =>
                {
                    validatorOpt.MustRevalidate = true;
                }
                
                );
        }

        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = 1,
                    Period = "5s"
                }
                //you can have mutiple rules here
            };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
