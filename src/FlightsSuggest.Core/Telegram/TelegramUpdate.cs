using System;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace FlightsSuggest.Core.Telegram
{
    public class TelegramUpdate
    {
        public TelegramUpdate(Update update)
        {
            if (update.Message == null)
            {
                throw new ArgumentException($"Message can't be null, update = {JsonConvert.SerializeObject(update, Formatting.Indented)}");
            }

            Text = update.Message.Text;
            Username = update.Message.Chat.Username;
            ChatId = update.Message.Chat.Id;
        }

        public string Text { get; }
        public string Username { get; }
        public long ChatId { get; }
    }
}