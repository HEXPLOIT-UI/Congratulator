using Congratulator.API.Filters;
using Congratulator.AppService.Files.Services;
using Congratulator.AppService.Validators.Commons;
using Congratulator.Infrastructure;
using Congratulator.Infrastructure.Configurations.Mappers;
using Congratulator.Infrastructure.Exstensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Host.UseSerilog(Log.Logger, true);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

builder.Services.AddControllers(opts =>
{
    opts.Filters.Add<ValidationFilter>();
}).AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddValidatorsFromAssemblyContaining<BaseRequestWithPaginationValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddOpenApi("v1");
builder.Services.ConfigureMapster();

#region DbConfig
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IPhotoStorageService, PhotoStorageService>();
#endregion

#region AuthenticationSettings
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.Name = "CongratulatorAPI";
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                };
            });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});
#endregion

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseCors("AllowSpecificOrigin");

await using (var scope = app.Services.CreateAsyncScope())
{
    var serviceProvider = scope.ServiceProvider;
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        logger.LogInformation("Database is up to date");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during database migration");
        throw;
    }
}

app.MapOpenApi("congratulatorapi/{documentName}.json");

app.MapScalarApiReference(
    configureOptions: opt =>
    {
        opt.Title = "Congratulator WebAPI Doc";
        opt.Theme = ScalarTheme.BluePlanet;
        opt.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.HttpClient);

        opt.OpenApiRoutePattern = "congratulatorapi/{documentName}.json";
    },
    endpointPrefix: "/congratulatorapiroute"
);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

namespace Congratulator.API
{
    public partial class Program { }
}