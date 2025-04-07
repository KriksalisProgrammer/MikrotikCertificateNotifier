# 📡 Mikrotik Certificate Notifier

A simple console application that connects to a MikroTik router, checks for certificates that are about to expire, and sends notifications via Telegram.

## 🚀 Features

- ✅ Connects to MikroTik using the [tik4net](https://github.com/danikf/tik4net) library
- 📅 Checks expiration dates of installed certificates
- ⚠️ Sends alerts via Telegram if a certificate is close to expiring
- 🛠 Fully configurable via `appsettings.json`

## 🔧 Configuration

Create an `appsettings.json` file in the root of the project with the following structure:

```json
{
  "Telegram": {
    "BotToken": "your_telegram_bot_token",
    "ChatId": "your_chat_id"
  },
  "Mikrotik": {
    "Host": "192.168.88.1",
    "User": "admin",
    "Password": "your_password"
  },
  "Notification": {
    "DaysBeforeExpiration": 30
  }
}
```
You can override any settings in a separate appsettings.local.json file (useful for secrets and local development).
## 💬 Telegram Setup

   1. Create a bot via @BotFather

   2. Get your bot token

   3. Start a chat with your bot

   4. Use @userinfobot to get your chat ID
## 📦 Dependencies

    .NET 6+

    tik4net

    Newtonsoft.Json

## ✅ How to Use

dotnet build
dotnet run

## 🧾 Use Cases

    Monitor client VPN or SSTP certificate validity

    Prevent unexpected downtime due to expired certificates

    Automate routine checks via scheduled task or cron
## 📌 License

This project is licensed under the MIT License.
