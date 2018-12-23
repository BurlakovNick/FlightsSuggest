using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;
using FlightsSuggest.Dto;

namespace FlightsSuggest.AzureFunctions.Implementation.Factories
{
    public class SubscriberDtoFactory : ISubscriberDtoFactory
    {
        private readonly ITelegramClient telegramClient;

        public SubscriberDtoFactory(
            ITelegramClient telegramClient
            )
        {
            this.telegramClient = telegramClient;
        }

        public Task<SubscriberDto[]> CreateAsync(Subscriber[] subscribers)
        {
            return Task.WhenAll(subscribers.Select(CreateAsync));
        }

        public async Task<SubscriberDto> CreateAsync(Subscriber subscriber)
        {
            User user = null;
            if (subscriber.TelegramChatId.HasValue && subscriber.TelegramUserId.HasValue)
            {
                user = await telegramClient.GetUserAsync(subscriber.TelegramChatId.Value, subscriber.TelegramUserId.Value);
            }

            return new SubscriberDto
            {
                Id = subscriber.Id,
                SendTelegramMessages = subscriber.SendTelegramMessages,
                TelegramChatId = subscriber.TelegramChatId.Value,
                TelegramUsername = subscriber.TelegramUsername,
                NotificationTrigger = subscriber.NotificationTrigger.Serialize(),
                FirstName = user?.FirstName,
                LastName = user?.LastName,
                IsBot = user?.IsBot,
                TelegramUserId = user?.UserId
            };
        }
    }
}