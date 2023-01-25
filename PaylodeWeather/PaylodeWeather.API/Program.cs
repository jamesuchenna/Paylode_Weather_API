using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaylodeWeather.API.ExtensionsConfigs;
using PaylodeWeather.Core.Interfaces;
using PaylodeWeather.Domain.Model;
using PaylodeWeather.Extensions;
using PaylodeWeather.ExtensionsConfigs;
using PaylodeWeather.Infrastructure;
using PaylodWeather.Core.Services;
using Serilog;
using WeatherApi.Core.Services;
using WeatherApi.Core.Utilities;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
LogSettings.SetUpSerilog(configuration);

try
{
    Log.Information("Application is starting...");
    // Add services to the container.
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // Serilog
    builder.Services.AddSingleton(Log.Logger);

    // Configure DbContext
    builder.Services.AddDbContext<AppDbContext>(options => options.
            UseSqlServer(configuration.GetConnectionString("PaylodeWeatherApiDb")));

    // Add Identity
    builder.Services.AddIdentity<AppUser, IdentityRole>(x =>
    {
        x.Password.RequiredLength = 8;
        x.Password.RequireDigit = true;
        x.Password.RequireUppercase = true;
        x.Password.RequireLowercase = true;
        x.User.RequireUniqueEmail = true;
    })
       .AddEntityFrameworkStores<AppDbContext>()
       .AddDefaultTokenProviders();

    //Authentication and Authorization configurations
    builder.Services.AddAuthenticationExtension(configuration);
    builder.Services.AddAuthorizationExtension();


    builder.Services.AddControllers();
    builder.Services.AddAutoMapper(typeof(AppUserProfiles));
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerConfiguration();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(setupAction =>
        {
            setupAction.SwaggerEndpoint("/swagger/WeatherOpenAPI/swagger.json", "Weather API");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception e)
{
    Log.Fatal(e.Message, e.StackTrace, "The application failed to start correctly");
}
finally
{
    Log.CloseAndFlush();
}