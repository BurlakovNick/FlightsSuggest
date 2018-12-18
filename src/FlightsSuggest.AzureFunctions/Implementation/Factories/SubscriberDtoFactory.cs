using System.Linq;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Dto;

namespace FlightsSuggest.AzureFunctions.Implementation.Factories
{
    public class SubscriberDtoFactory : ISubscriberDtoFactory
    {
        public SubscriberDto[] Create(Subscriber[] subscribers)
        {
            return subscribers.Select(Create).ToArray();
        }

        public SubscriberDto Create(Subscriber subscriber)
        {
            return new SubscriberDto
            {
                Id = subscriber.Id,
                SendTelegramMessages = subscriber.SendTelegramMessages,
                TelegramChatId = subscriber.TelegramChatId.Value,
                TelegramUsername = subscriber.TelegramUsername,
                NotificationTrigger = subscriber.NotificationTrigger.Serialize(),
            };
        }
    }
}