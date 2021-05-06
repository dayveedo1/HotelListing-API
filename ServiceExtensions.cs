using HotelListingAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
    }
}
