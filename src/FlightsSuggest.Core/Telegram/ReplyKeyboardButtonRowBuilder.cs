using System.Collections.Generic;

namespace FlightsSuggest.Core.Telegram
{
    public class ReplyKeyboardButtonRowBuilder
    {
        private readonly List<ReplyKeyboardButton> buttons;

        public ReplyKeyboardButtonRowBuilder()
        {
            buttons = new List<ReplyKeyboardButton>();
        }

        public ReplyKeyboardButtonRowBuilder AddButton(string text)
        {
            buttons.Add(new ReplyKeyboardButton(text));
            return this;
        }

        public ReplyKeyboardButton[] Build()
        {
            return buttons.ToArray();
        }
    }
}