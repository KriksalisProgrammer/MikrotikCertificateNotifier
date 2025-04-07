using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MickrotTelegramCertificat;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using tik4net;
using tik4net.Objects;  

namespace MikrotikCertificateNotifier
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Запуск проверки сертификатов Mikrotik...");

            try
            {

                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.local.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();


                var telegramSettings = config.GetSection("Telegram");
                var mikrotikSettings = config.GetSection("Mikrotik");
                var notificationSettings = config.GetSection("Notification");

                string botToken = telegramSettings["BotToken"];
                string chatId = telegramSettings["ChatId"];
                string host = mikrotikSettings["Host"];
                string user = mikrotikSettings["User"];
                string password = mikrotikSettings["Password"];
                int daysBeforeExpiry = int.TryParse(notificationSettings["DaysBeforeExpiration"], out int result) ? result : 30;

                if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId) ||
                    string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) ||
                    string.IsNullOrEmpty(password))
                {
                    throw new Exception("Отсутствуют необходимые настройки в конфигурационном файле");
                }

                Console.WriteLine("Подключение к Mikrotik...");

                using (ITikConnection connection = ConnectionFactory.CreateConnection(TikConnectionType.Api))
                {
                    connection.Open(host, user, password);

                    Console.WriteLine("Подключено успешно. Получение списка сертификатов...");

                    var certificates = connection.LoadList<Certificate>();

                    bool foundExpiringSoon = false;

                    foreach (var cert in certificates)
                    {
                        if (string.IsNullOrEmpty(cert.InvalidAfter))
                            continue;

                        if (DateTime.TryParse(cert.InvalidAfter, out DateTime expiryDate))
                        {
                            int daysLeft = (int)(expiryDate - DateTime.Now).TotalDays;

                            if (daysLeft > 0 && daysLeft <= daysBeforeExpiry)
                            {
                                foundExpiringSoon = true;
                                string certDisplayName = !string.IsNullOrEmpty(cert.CommonName)
                                    ? $"{cert.Name} ({cert.CommonName})"
                                    : cert.Name;

                                string message = $"⚠️ Сертификат в MikroTik заканчивается: '{certDisplayName}'\n" +
                                                 $"Осталось дней: {daysLeft}\n" +
                                                 $"Дата окончания: {expiryDate:dd.MM.yyyy}";

                                Console.WriteLine(message);
                                await SendTelegramMessageAsync(botToken, chatId, message);
                            }
                        }
                    }

                    if (!foundExpiringSoon)
                    {
                        Console.WriteLine("Истекающих сертификатов не найдено");
                    }

                    connection.Close();
                    Console.WriteLine("Проверка завершена");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
            }
        }

        private static async Task SendTelegramMessageAsync(string botToken, string chatId, string message)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

                    var payload = new
                    {
                        chat_id = chatId,
                        text = message,
                        parse_mode = "HTML"
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine("Сообщение в Telegram отправлено успешно");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке в Telegram: {ex.Message}");
            }
        }
    }
}
