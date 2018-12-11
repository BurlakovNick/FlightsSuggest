using System;

namespace FlightsSuggest.Admin.Infrastructure
{
    public class SubscriberDto
    {
        public Guid Id { get; set; }
        public bool SendTelegramMessages { get; set; }
        public int TelegramChatId { get; set; }
        public string TelegramUsername { get; set; }
        public string NotificationTrigger { get; set; }
    }
}