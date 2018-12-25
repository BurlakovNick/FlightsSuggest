using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation.Storage;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public class SetUpSearchCommand : ICommand
    {
        private readonly INotificationTrigger notificationTrigger;
        private readonly ISubscriberStorage subscriberStorage;
        private readonly ITelegramClient telegramClient;

        public SetUpSearchCommand(
            INotificationTrigger notificationTrigger,
            ISubscriberStorage subscriberStorage,
            ITelegramClient telegramClient
        )
        {
            this.notificationTrigger = notificationTrigger;
            this.subscriberStorage = subscriberStorage;
            this.telegramClient = telegramClient;
        }

        public CommandType Type => CommandType.SetUpSearch;

        public async Task ExecuteAsync(Subscriber subscriber, long chatId)
        {
            await subscriberStorage.UpdateNotificationTriggerAsync(subscriber.Id, notificationTrigger);
            await telegramClient.SendMessageAsync(chatId, "Принято, босс! Теперь бот будет присылать только новости с такими словами");
        }
    }
}