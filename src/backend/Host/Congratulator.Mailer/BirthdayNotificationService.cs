using Congratulator.AppService.Base;
using Congratulator.AppService.Users.Repositories;
using Congratulator.Domain.Users.Entity;
using Congratulator.Mailer.Services.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Congratulator.Mailer;

public class BirthdayNotificationService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BirthdayNotificationService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(12);

    public BirthdayNotificationService(IServiceProvider services, ILogger<BirthdayNotificationService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_checkInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SendBirthdayNotificationsAsync(stoppingToken);
        }
    }

    private async Task SendBirthdayNotificationsAsync(CancellationToken ct)
    {
        await using var scope = _services.CreateAsyncScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var notifier = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var users = (IUserRepository)uow.Repository<UserEntity>();
        var today = DateTime.Today.ToUniversalTime();

        await foreach (var user in users.GetFiltered(x => !string.IsNullOrWhiteSpace(x.TelegramId)).Include(x => x.Birthdays).AsAsyncEnumerable().WithCancellation(ct))
        {
            var birthdaysToday = new List<string>();
            var birthdaysIn7Days = new List<string>();

            foreach (var bd in user.Birthdays)
            {
                var month = bd.DateOfBirth.Month;
                var day = bd.DateOfBirth.Day;
                var nextBirthday = new DateTime(today.Year, month, day);
                if (nextBirthday < today)
                    nextBirthday = nextBirthday.AddYears(1);

                var daysUntil = (nextBirthday - today).Days;

                if (daysUntil == 0)
                {
                    birthdaysToday.Add($"{bd.FirstName} {bd.LastName} ({day:00}.{month:00})");
                }
                else if (daysUntil <= 7)
                {
                    birthdaysIn7Days.Add($"{bd.FirstName} {bd.LastName} ({day:00}.{month:00})");
                }
            }

            if (birthdaysToday.Any() || birthdaysIn7Days.Any())
            {
                var messageBuilder = new StringBuilder();

                if (birthdaysToday.Any())
                {
                    messageBuilder.AppendLine("🎂 Дни рождения сегодня:");
                    foreach (var s in birthdaysToday)
                        messageBuilder.AppendLine($" - {s}");
                    messageBuilder.AppendLine();
                }

                if (birthdaysIn7Days.Any())
                {
                    messageBuilder.AppendLine("⏳ Ближайшие дни рождения:");
                    foreach (var s in birthdaysIn7Days)
                        messageBuilder.AppendLine($" - {s}");
                }

                var message = messageBuilder.ToString().TrimEnd();

                await notifier.SendNotificationAsync(user, message, ct);
            }
        }
    }
}