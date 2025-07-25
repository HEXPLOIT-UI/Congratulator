using Congratulator.Domain.Users.Entity;
using Congratulator.Mailer.Services.Base;
using Microsoft.Extensions.Configuration;

namespace Congratulator.Mailer.Services;

public class TelegramNotificationService : INotificationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _botToken;

    public TelegramNotificationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _botToken = configuration["TELEGRAM_BOT_TOKEN"]
            ?? throw new ArgumentNullException("TELEGRAM_BOT_TOKEN не задан в конфигурации");
    }

    public Task SendNotificationAsync(UserEntity user, string message, CancellationToken ct = default)
    {
        return SendMessageAsync(user.TelegramId!, message, ct);
    }

    private async Task SendMessageAsync(string chatId, string text, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient();
        var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
        using var formData = new MultipartFormDataContent
        {
            { new StringContent(chatId), "chat_id" },
            { new StringContent(text), "text" }
        };

        var response = await client.PostAsync(url, formData, ct);
        response.EnsureSuccessStatusCode();
    }
}
