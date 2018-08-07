using System.Linq;
using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public class Subscriber
    {
        private readonly INotificationTrigger[] notificationTriggers;

        public Subscriber(
            string id,
            string telegramUsername,
            long? telegramChatId,
            bool sendTelegramMessages,
            INotificationTrigger[] notificationTriggers
            )
        {
            this.notificationTriggers = notificationTriggers;
            Id = id;
            TelegramUsername = telegramUsername;
            SendTelegramMessages = sendTelegramMessages;
            TelegramChatId = telegramChatId;
        }

        public string Id { get; }
        public string TelegramUsername { get; }
        public bool SendTelegramMessages { get; }
        public long? TelegramChatId { get; }

        public bool ShouldNotify(FlightNews flightNews)
        {
            return notificationTriggers.Any(x => x.ShouldNotify(flightNews));
        }
    }
}