namespace FlightsSuggest.Core.Telegram
{
    public class ReplyKeyboardButton
    {
        public ReplyKeyboardButton(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}