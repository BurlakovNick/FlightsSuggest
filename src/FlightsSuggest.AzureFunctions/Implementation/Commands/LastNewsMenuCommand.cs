using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public class LastNewsMenuCommand : ICommand
    {
        private readonly IFlightsConfiguration configuration;
        private readonly ITelegramClient telegramClient;

        public LastNewsMenuCommand(
            IFlightsConfiguration configuration,
            ITelegramClient telegramClient
        )
        {
            this.configuration = configuration;
            this.telegramClient = telegramClient;
        }

        public CommandType Type => CommandType.LastNewsMenu;

        public async Task ExecuteAsync(Subscriber subscriber, long chatId)
        {
            var replyKeyboard = new[] { 1, 3, 7, 14 }
                .Aggregate(new ReplyKeyboardBuilder(), (builder, x) =>
                {
                    var buttonText = PatternParser.ReplacePatternWithInt(configuration.TelegramLastNewsFormat, x);
                    return builder.AddRow(new ReplyKeyboardButton(buttonText));
                })
                .AddRow(new ReplyKeyboardButton("Обратно!"));

            await telegramClient.SendMessageAsync(
                chatId,
                "За какой период будем искать?",
                replyKeyboard);
        }
    }
}