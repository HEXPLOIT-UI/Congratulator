using Congratulator.Models;
using Congratulator.Repositories;

namespace Congratulator.Services.Hosted
{
    public class BotBackgroundService : BackgroundService
    {
        private readonly string m_daysToNotification;
        private readonly string m_token;
        private readonly string m_chatId;
        private readonly BirthdayRepository m_birthdayRepository;
        private readonly HttpClient m_client;
        
        public BotBackgroundService(IConfiguration configuration, BirthdayRepository birthdayRepository)
        {
            m_daysToNotification = configuration["mailing:days_to_notification"] ?? "";
            m_token = configuration["mailing:bot_token"] ?? "";
            m_chatId = configuration["mailing:chat_id"] ?? "";
            m_birthdayRepository = birthdayRepository;
            m_client = new HttpClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                var todayBirthdays = m_birthdayRepository.GetTodayBirthdays();
                if (todayBirthdays.Count() == 0)
                {
                    return;
                }
                else
                {
                    await SendMessage("Сегодня день рождение у: ");
                    foreach (BirthdayModel bm in todayBirthdays)
                    {
                        string profileLinkUri = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", bm.Id.ToString()+".jpg");

                        await SendPhoto($"{bm.FullName}", File.Exists(profileLinkUri) ? profileLinkUri : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "default.jpg"));
                    }
                }
                var birthdaysToNotificate = m_birthdayRepository.getBirthdaysToNotificate(int.Parse(m_daysToNotification));
                if (birthdaysToNotificate.Count() != 0)
                {

                    await SendMessage($"Также через {m_daysToNotification} дня(дней) будут дни рождения у: ");
                    foreach (BirthdayModel bm in birthdaysToNotificate)
                    {
                      
                        string profileLinkUri = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", bm.Id.ToString() + ".jpg");
                        await SendPhoto($"{bm.FullName}", File.Exists(profileLinkUri) ? profileLinkUri : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "default.jpg"));
                    }

                }
                await Task.Delay(86400 * 1000, stoppingToken);
            }
        }

        private async Task SendMessage(string text)
        {
            string url = $"https://api.telegram.org/bot{m_token}/sendMessage";
            MultipartFormDataContent formData = new MultipartFormDataContent
            {
                { new StringContent(m_chatId.ToString()), "chat_id" },
                { new StringContent(text), "text" }
            };
            HttpResponseMessage response = await m_client.PostAsync(url, formData);
            /*string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);*/
        }

        private async Task SendPhoto(string text, string photoLink)
        {
            string url = $"https://api.telegram.org/bot{m_token}/sendPhoto";

            MultipartFormDataContent formData = new MultipartFormDataContent
            {
                { new StringContent(m_chatId.ToString()), "chat_id" },
                { new StringContent(text), "caption" }
            };

            if (!string.IsNullOrEmpty(photoLink) && File.Exists(photoLink))
            {
                byte[] photoBytes = File.ReadAllBytes(photoLink);
                formData.Add(new ByteArrayContent(photoBytes), "photo", "photo.jpg");
            } 

            HttpResponseMessage response = await m_client.PostAsync(url, formData);
            /*string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);*/
        }
    }
}
