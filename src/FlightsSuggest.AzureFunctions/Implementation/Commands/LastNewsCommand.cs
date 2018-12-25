using System;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;
using FlightsSuggest.Core.Timelines;
using Microsoft.Extensions.Logging;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public class LastNewsCommand : ICommand
    {
        private readonly int lastNewsDays;
        private readonly IFlightsConfiguration configuration;
        private readonly ITelegramClient telegramClient;
        private readonly INotifier notifier;
        private readonly ITimeline[] timelines;
        private readonly ILogger log;

        public LastNewsCommand(
            int lastNewsDays,
            IFlightsConfiguration configuration,
            ITelegramClient telegramClient,
            INotifier notifier,
            ITimeline[] timelines,
            ILogger log
        )
        {
            this.lastNewsDays = lastNewsDays;
            this.configuration = configuration;
            this.telegramClient = telegramClient;
            this.notifier = notifier;
            this.timelines = timelines;
            this.log = log;
        }

        public CommandType Type => CommandType.LastNews;

        public async Task ExecuteAsync(Subscriber subscriber, long chatId)
        {
            var newDate = DateTime.UtcNow.AddDays(-lastNewsDays);
            var newOffset = newDate.Ticks;

            foreach (var timeline in timelines)
            {
                log.LogInformation($"Rewinding {subscriber.TelegramUsername} offset in {timeline.Name} timeline to {newDate}");
                await notifier.RewindOffsetAsync(subscriber.Id, timeline.Name, newOffset);
                await notifier.NotifyAsync(subscriber);
            }

            await telegramClient.SendMessageAsync(chatId, "Сделано, хозяин!");
        }
    }
}