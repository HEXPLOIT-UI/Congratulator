using Congratulator.Infrastructure.Exstensions;
using Congratulator.Mailer;
using Congratulator.Mailer.Services;
using Congratulator.Mailer.Services.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(config =>
{
    config
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);

    config.AddUserSecrets<Program>();
    config.AddEnvironmentVariables();
}).ConfigureServices((hostingContext, services) =>
{
    services.AddInfrastructure(hostingContext.Configuration);
    services.AddTransient<INotificationService, TelegramNotificationService>();
    services.AddHostedService<BirthdayNotificationService>();

    services.AddHttpClient();
})
.Build();
await host.RunAsync();