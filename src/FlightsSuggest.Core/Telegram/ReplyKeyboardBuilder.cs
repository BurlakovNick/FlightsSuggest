using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlightsSuggest.Core.Telegram
{
    public class ReplyKeyboardBuilder
    {
        private readonly bool resizeKeyboard;
        private readonly List<ReplyKeyboardButtonRow> rows;

        public ReplyKeyboardBuilder(bool resizeKeyboard = true)
        {
            this.resizeKeyboard = resizeKeyboard;
            rows = new List<ReplyKeyboardButtonRow>();
        }

        public ReplyKeyboardBuilder AddRow(ReplyKeyboardButtonRow row)
        {
            rows.Add(row);
            return this;
        }

        public ReplyKeyboardBuilder AddRow(ReplyKeyboardButton button)
        {
            rows.Add(new ReplyKeyboardButtonRow {Buttons = new[] {button}});
            return this;
        }

        public ReplyKeyboardMarkup Build()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = rows
                    .Select(row => row
                        .Buttons
                        .Select(button => new KeyboardButton(button.Text))
                        .ToArray())
                    .ToArray(),
                ResizeKeyboard = resizeKeyboard
            };
        }
    }
}