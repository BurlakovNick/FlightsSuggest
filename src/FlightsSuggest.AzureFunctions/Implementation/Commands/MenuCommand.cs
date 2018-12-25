using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public class MenuCommand : ICommand
    {
        private readonly IFlightsConfiguration configuration;
        private readonly ITelegramClient telegramClient;

        public MenuCommand(
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
            var menuKeyboard = new ReplyKeyboardBuilder()
                .AddRow(new ReplyKeyboardButton(configuration.TelegramSearchSettingRequestWords))
                .AddRow(new ReplyKeyboardButton(configuration.TelegramLastNewsRequestFormat));

            await telegramClient.SendMessageAsync(
                chatId,
                "Этот бот поможет тебе найти дешевые билеты в интернетах. Что сделать для тебя, дружище?",
                menuKeyboard);
        }
    }
}