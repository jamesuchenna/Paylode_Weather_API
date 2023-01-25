using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PaylodeWeather.Domain.Model;
using System.Text;
using PaylodeWeather.Infrastructure;

namespace PaylodeWeather.Extensions
{
    public static class AuthenticationConfiguration
    {
        public static void AddAuthenticationExtension(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("JWTSettings");
            services.AddAuthentication(v =>
            {
                v.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                v.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(v =>
             {
                 v.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateLifetime = true,
                     ValidateAudience = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwtConfig.GetSection("Issuer").Value,
                     ValidAudience = jwtConfig.GetSection("Audience").Value,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWTSettings:Key")))
                 };
             });

            services.AddIdentity<AppUser, IdentityRole>(x =>
            {
                x.Password.RequiredLength = 8;
                x.Password.RequireDigit = true;
                x.Password.RequireUppercase = true;
                x.Password.RequireLowercase = true;
                x.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}