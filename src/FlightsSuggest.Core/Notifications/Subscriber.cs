using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public class Subscriber
    {
        public Subscriber(
            string id,
            string telegramUsername,
            long? telegramChatId,
            bool sendTelegramMessages,
            INotificationTrigger notificationTrigger
            )
        {
            Id = id;
            NotificationTrigger = notificationTrigger;
            TelegramUsername = telegramUsername;
            SendTelegramMessages = sendTelegramMessages;
            TelegramChatId = telegramChatId;
        }


        public string Id { get; }
        public string TelegramUsername { get; }
        public bool SendTelegramMessages { get; }
        public long? TelegramChatId { get; }
        public INotificationTrigger NotificationTrigger { get; }

        public bool ShouldNotify(FlightNews flightNews)
        {
            return NotificationTrigger.ShouldNotify(flightNews);
        }
    }
}