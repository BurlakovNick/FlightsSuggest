using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public class Notifier
    {
        private readonly INotificationSender[] senders;
        private readonly Subscriber[] subscribers;
        private readonly ITimeline[] timelines;
        private readonly IOffsetStorage offsetStorage;

        public Notifier(
            INotificationSender[] senders,
            Subscriber[] subscribers,
            ITimeline[] timelines,
            IOffsetStorage offsetStorage
        )
        {
            this.senders = senders;
            this.subscribers = subscribers;
            this.timelines = timelines;
            this.offsetStorage = offsetStorage;
        }

        public async Task NotifyAsync()
        {
            foreach (var subscriber in subscribers)
            {
                foreach (var notificationSender in senders.Where(s => s.CanSend(subscriber)))
                {
                    foreach (var timeline in timelines)
                    {
                        var offsetId = $"{subscriber.Id}_{timeline.Name}";
                        var offset = await offsetStorage.FindAsync(offsetId);
                        if (offset == null)
                        {
                            var latestOffset = await timeline.GetLatestOffsetAsync();
                            if (latestOffset.HasValue)
                            {
                                await offsetStorage.WriteAsync(offsetId, latestOffset.Value);
                            }
                            continue;
                        }

                        var flightEnumerator = timeline.GetNewsEnumerator(offset.Value);
                        while (true)
                        {
                            var (hasNext, flightNews) = await flightEnumerator.MoveNextAsync();
                            if (!hasNext)
                            {
                                break;
                            }

                            if (subscriber.ShouldNotify(flightNews))
                            {
                                notificationSender.SendTo(subscriber, flightNews);
                            }
                        }
                    }
                }
            }
        }
    }
}