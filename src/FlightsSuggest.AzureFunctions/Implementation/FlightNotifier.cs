using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure.Vkontakte;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class FlightNotifier
    {
        private readonly IFlightsConfiguration configuration;
        private readonly VkontakteClient vkontakteClient;
        private VkontakteTimeline vkontakteTimeline;
        private Notifier notifier;
        private INotificationSender telegramNotificationSender;
        private SubscriberStorage subscriberStorage;
        private AzureTableOffsetStorage offsetStorage;

        public FlightNotifier(
            IFlightsConfiguration configuration
            )
        {
            this.configuration = configuration;
            vkontakteClient = new VkontakteClient(configuration.VkApplicationId, configuration.VkAccessToken);
            Init();
        }

        public FlightNews[] Sended { get; private set; }

        public async Task NotifyAsync()
        {
            await vkontakteTimeline.ActualizeAsync();
            await notifier.NotifyAsync();
            Sended = telegramNotificationSender.Sended;
        }

        public Task RewindSubscriberOffsetAsync(string subscriberId, string timelineName, long offset)
        {
            return notifier.RewindOffsetAsync(subscriberId, timelineName, offset);
        }

        public async Task RewindVkOffsetAsync(string vkGroup, long offset)
        {
            var timeline = new [] {vkontakteTimeline}.FirstOrDefault(x => x.VkGroupName == vkGroup);
            if (timeline == null)
            {
                return;
            }

            await timeline.WriteOffsetAsync(offset);
        }

        public Task<Subscriber[]> SelectSubscribersAsync()
        {
            return subscriberStorage.SelectAllAsync();
        }

        public Task<(string timelineName, DateTime? offset)[]> SelectOffsetsAsync(string subscriberId)
        {
            return notifier.SelectOffsetsAsync(subscriberId);
        }

        public async Task<(string vkGroupName, DateTime? offset)[]> SelectVkOffsetsAsync()
        {
            var timelines = new [] { vkontakteTimeline};
            var result = new List<(string, DateTime?)>();
            foreach (var timeline in timelines)
            {
                var offset = await timeline.GetLatestOffsetAsync();
                if (offset.HasValue)
                {
                    result.Add((timeline.VkGroupName, new DateTime(offset.Value)));
                }
                else
                {
                    result.Add((timeline.VkGroupName, null));
                }
            }

            return result.ToArray();
        }

        public Task CreateSubscriberAsync(string telegramUsername)
        {
            return subscriberStorage.CreateAsync(telegramUsername);
        }

        public async Task ProcessTelegramUpdateAsync(Update update, TraceWriter log)
        {
            log.Info($"Received update: {JsonConvert.SerializeObject(update)}");

            if (update.Message?.Text == null)
            {
                log.Info("Message text is null, quiting");
                return;
            }

            if (update.Message.Text.ToLower().Contains(configuration.TelegramMagicWords))
            {
                log.Info("Seen magic words, creating new subscriber");

                var telegramUsername = update.Message.Chat.Username;
                var telegramChatId = update.Message.Chat.Id;

                var subscribers = (await subscriberStorage.SelectAllAsync()).ToDictionary(x => x.TelegramUsername);

                if (subscribers.TryGetValue(telegramUsername, out var subscriber) &&
                    subscriber.TelegramChatId.HasValue)
                {
                    log.Info("Subscriber already created");
                    return;
                }

                if (subscriber == null)
                {
                    subscriber = await subscriberStorage.CreateAsync(telegramUsername);
                }

                await subscriberStorage.UpdateTelegramChatIdAsync(subscriber.Id, telegramChatId);
                return;
            }
        }

        private void Init()
        {
            offsetStorage = new AzureTableOffsetStorage(configuration);

            vkontakteTimeline = new VkontakteTimeline(
                "vandroukiru",
                offsetStorage,
                new AzureTableFlightNewsStorage(configuration), 
                vkontakteClient,
                new FlightNewsFactory()
            );

            telegramNotificationSender = new TelegramNotificationSender(configuration);
            subscriberStorage = new SubscriberStorage(configuration);

            var notificationSenders = new[] { telegramNotificationSender, };
            var subscribers = subscriberStorage.SelectAllAsync().Result;

            notifier = new Notifier(notificationSenders, subscribers, new[] { vkontakteTimeline }, offsetStorage);
        }
    }
}