namespace FlightsSuggest.Dto
{
    public class SubscriberDto
    {
        public string Id { get; set; }
        public bool SendTelegramMessages { get; set; }
        public long TelegramChatId { get; set; }
        public string TelegramUsername { get; set; }
        public string NotificationTrigger { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsBot { get; set; }
        public int? TelegramUserId { get; set; }
    }
}
