namespace FlightsSuggest.Dto
{
    public class SubscriberDto
    {
        public string Id { get; set; }
        public bool SendTelegramMessages { get; set; }
        public long TelegramChatId { get; set; }
        public string TelegramUsername { get; set; }
        public string NotificationTrigger { get; set; }
    }
}
