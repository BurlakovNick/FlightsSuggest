namespace FlightsSuggest.Core.Telegram
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public bool IsBot { get; set; }
    }
}