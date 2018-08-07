using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure.Vkontakte;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;
using Telegram.Bot;

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

        public async Task<string[]> RegisterNewSubscribers()
        {
            var botClient = new TelegramBotClient(configuration.TelegramBotToken);

            const string botOffsetKey = "telegram_bot_offset";
            var telegramBotOffset = ((int?)await offsetStorage.FindAsync(botOffsetKey)) ?? 0;
            var subscribers = (await subscriberStorage.SelectAllAsync()).ToDictionary(x => x.TelegramUsername);
            var updated = new List<string>();

            while (true)
            {
                var updates = await botClient.GetUpdatesAsync(telegramBotOffset, limit: 100);
                if (updates.Length == 0)
                {
                    break;
                }

                foreach (var update in updates
                    .Where(x => x.Message?.Text != null)
                    .Where(x => x.Message.Text.ToLower().Contains(configuration.TelegramMagicWords))
                )
                {
                    var telegramUsername = update.Message.Chat.Username;
                    var telegramChatId = update.Message.Chat.Id;

                    if (subscribers.TryGetValue(telegramUsername, out var subscriber) &&
                        subscriber.TelegramChatId.HasValue)
                    {
                        continue;
                    }

                    if (subscriber == null)
                    {
                        subscriber = await subscriberStorage.CreateAsync(telegramUsername);
                    }

                    subscriber = await subscriberStorage.UpdateTelegramChatIdAsync(subscriber.Id, telegramChatId);
                    updated.Add(telegramUsername);
                }

                telegramBotOffset = updates.Last().Id + 1;
            }

            await offsetStorage.WriteAsync(botOffsetKey, telegramBotOffset);

            return updated.ToArray();
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