namespace FlightsSuggest.Admin.Models
{
    public class SubscriberListViewModel
    {
        public SubscriberViewModel[] Subscribers { get; set; }
    }

    public class SubscriberViewModel
    {
        public string Id { get; set; }
        public string TelegramUsername { get; set; }
        public string TelegramName { get; set; }
        public bool SendTelegramMessages { get; set; }
        public long? TelegramChatId { get; set; }
        public string NotificationTrigger { get; set; }
    }
}