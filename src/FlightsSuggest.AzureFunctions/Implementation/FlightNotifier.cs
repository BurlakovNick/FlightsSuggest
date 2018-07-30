using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure.Vkontakte;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class FlightNotifier
    {
        private readonly IFlightsConfiguration configuration;
        private readonly VkontakteClient vkontakteClient;
        private VkontakteTimeline vkontakteTimeline;
        private Notifier notifier;
        private InMemoryNotificationSender inMemoryNotificationSender;

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
            Sended = inMemoryNotificationSender.Sended;
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

        private void Init()
        {
            var offsetStorage = new AzureTableOffsetStorage(configuration);

            vkontakteTimeline = new VkontakteTimeline(
                "vandroukiru",
                offsetStorage,
                new AzureTableFlightNewsStorage(configuration), 
                vkontakteClient,
                new FlightNewsFactory()
            );

            inMemoryNotificationSender = new InMemoryNotificationSender();
            var notificationSenders = new[] { inMemoryNotificationSender, };
            var subscriber = new Subscriber("nick", null, false, new[] { new TermNotificationTrigger("Греци"), });
            notifier = new Notifier(notificationSenders, new[] { subscriber, }, new[] { vkontakteTimeline }, offsetStorage);
        }
    }
}