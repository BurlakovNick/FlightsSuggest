using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public class SetUpSearchMenuCommand : ICommand
    {
        private readonly ITelegramClient telegramClient;
        private readonly IFlightsConfiguration configuration;

        public SetUpSearchMenuCommand(
            ITelegramClient telegramClient,
            IFlightsConfiguration configuration
        )
        {
            this.telegramClient = telegramClient;
            this.configuration = configuration;
        }

        public CommandType Type => CommandType.SetUpSearch;

        public async Task ExecuteAsync(Subscriber subscriber, long chatId)
        {
            await telegramClient.SendMessageAsync(chatId,
                "Чтобы настроить поиск, нужно послать боту сообщение в таком формате:");

            await telegramClient.SendMessageAsync(chatId,
                $"{configuration.TelegramSearchSettingWords} [Дублин, Майорка, Вена]");

            await telegramClient.SendMessageAsync(chatId,
                "Бот будет искать новости, в тексте который есть слова 'Дублин', 'Майорка' или 'Вена'");

            await telegramClient.SendMessageAsync(chatId,
                "Если хочешь, чтобы обязательно встретилось несколько слов, можешь обернуть их в круглые скобки:");

            await telegramClient.SendMessageAsync(chatId,
                $"{configuration.TelegramSearchSettingWords} (Тайланд, дешево)");

            await telegramClient.SendMessageAsync(chatId,
                "Условия можно комбинировать как тебе захочется! Например, если хочешь полететь в Тайланд дешево или в Дублин дорого, сделай так:");

            await telegramClient.SendMessageAsync(chatId,
                $"{configuration.TelegramSearchSettingWords} [(Тайланд, дешево), (Дублин, дорого)]");
        }
    }
}