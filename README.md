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
