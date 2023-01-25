using Microsoft.Extensions.DependencyInjection;
using PaylodeWeather.Domain.Enums;

namespace PaylodeWeather.Infrastructure
{
    public static class AuthorizationConfiguration
    {
        public static void AddAuthorizationExtension(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminOnly", policy => policy.RequireRole(UserRole.Admin.ToString()));
                options.AddPolicy("RequireCustomerOnly", policy => policy.RequireRole(UserRole.Customer.ToString()));
            });
        }
    }
}