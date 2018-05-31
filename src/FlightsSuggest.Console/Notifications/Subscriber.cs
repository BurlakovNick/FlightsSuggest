using System.Linq;
using FlightsSuggest.ConsoleApp.Timelines;

namespace FlightsSuggest.ConsoleApp.Notifications
{
    public class Subscriber
    {
        private readonly INotificationTrigger[] notificationTriggers;

        public Subscriber(
            string id,
            string telegramUsername,
            bool sendTelegramMessages,
            INotificationTrigger[] notificationTriggers
            )
        {
            this.notificationTriggers = notificationTriggers;
            Id = id;
            TelegramUsername = telegramUsername;
            SendTelegramMessages = sendTelegramMessages;
        }

        public string Id { get; }
        public string TelegramUsername { get; }
        public bool SendTelegramMessages { get; }

        public bool ShouldNotify(FlightNews flightNews)
        {
            return notificationTriggers.Any(x => x.ShouldNotify(flightNews));
        }
    }
}