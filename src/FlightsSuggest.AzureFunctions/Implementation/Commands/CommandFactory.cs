using FlightsSuggest.AzureFunctions.Implementation.Storage;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;
using FlightsSuggest.Core.Timelines;
using Microsoft.Extensions.Logging;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IFlightsConfiguration configuration;
        private readonly ITelegramClient telegramClient;
        private readonly INotifier notifier;
        private readonly ITimeline[] timelines;
        private readonly ILogger log;
        private readonly ISubscriberStorage subscriberStorage;

        public CommandFactory(
            IFlightsConfiguration configuration,
            ITelegramClient telegramClient,
            INotifier notifier,
            ITimeline[] timelines,
            ILogger log,
            ISubscriberStorage subscriberStorage
            )
        {
            this.configuration = configuration;
            this.telegramClient = telegramClient;
            this.notifier = notifier;
            this.timelines = timelines;
            this.log = log;
            this.subscriberStorage = subscriberStorage;
        }

        public ICommand CreateSetUpSearch(INotificationTrigger notificationTrigger)
        {
            return new SetUpSearchCommand(notificationTrigger, subscriberStorage, telegramClient);
        }

        public ICommand CreateSetUpSearchMenu()
        {
            return new SetUpSearchMenuCommand(telegramClient, configuration);
        }

        public ICommand CreateLastNewsMenu()
        {
            return new LastNewsMenuCommand(configuration, telegramClient);
        }

        public ICommand CreateLastNews(int lastNewsDays)
        {
            return new LastNewsCommand(lastNewsDays, configuration, telegramClient, notifier, timelines, log);
        }

        public ICommand CreateMenu()
        {
            return new MenuCommand(configuration, telegramClient);
        }
    }
}