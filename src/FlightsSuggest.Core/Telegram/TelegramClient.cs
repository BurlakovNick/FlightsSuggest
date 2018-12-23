using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsSuggest.Core.Telegram
{
    public class TelegramClient : ITelegramClient
    {
        private readonly ILogger logger;
        private readonly TelegramBotClient telegramBotClient;

        public TelegramClient(
            IFlightsConfiguration configuration,
            ILogger logger
            )
        {
            this.logger = logger;
            telegramBotClient = new TelegramBotClient(configuration.TelegramBotToken);
        }

        public Task SendMessageAsync(long chatId, string text, ReplyKeyboardBuilder replyKeyboard = null)
        {
            logger.LogInformation($"Sending to chat {chatId} message {text}");
            return telegramBotClient.SendTextMessageAsync(new ChatId(chatId), text, replyMarkup: replyKeyboard?.Build());
        }

        public async Task<User> GetUserAsync(long chatId, int userId)
        {
            logger.LogInformation($"Get user from chat {chatId}, userId {userId}");
            
            var user = await telegramBotClient.GetChatMemberAsync(new ChatId(chatId), userId);
            return new User
            {
                UserId = userId,
                FirstName = user.User.FirstName,
                LastName = user.User.LastName,
                Username = user.User.Username,
                IsBot = user.User.IsBot,
            };
        }
    }
}