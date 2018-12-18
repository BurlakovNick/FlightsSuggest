using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsSuggest.Core.Telegram
{
    public class TelegramClient : ITelegramClient
    {
        private readonly TelegramBotClient telegramBotClient;

        public TelegramClient(IFlightsConfiguration configuration)
        {
            telegramBotClient = new TelegramBotClient(configuration.TelegramBotToken);
        }

        public Task SendMessageAsync(long chatId, string text, ReplyKeyboardBuilder replyKeyboard = null)
        {
            return telegramBotClient.SendTextMessageAsync(new ChatId(chatId), text, replyMarkup: replyKeyboard?.Build());
        }
    }
}