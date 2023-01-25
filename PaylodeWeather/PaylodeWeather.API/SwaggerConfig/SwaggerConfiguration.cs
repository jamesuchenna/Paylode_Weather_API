using Microsoft.OpenApi.Models;

namespace PaylodeWeather.SwaggerConfig
{
    public static class SwaggerConfigurationExtension
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("WeatherOpenAPI", new()
                {
                    Title = "Weather API",
                    Version = "1",
                    Description = "A web API to get weather forcast",
                    Contact = new OpenApiContact
                    {
                        Name = "James Uchenna",
                        Email = "jamesuchennachi@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/james-uchenna/"),
                    }
                });

                setupAction.IncludeXmlComments(string.Format(@"{0}\WeatherApi.xml", Directory.GetCurrentDirectory()));

                // To Enable authorization using Swagger (JWT) 
                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
