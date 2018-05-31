using System.Linq;
using FlightsSuggest.ConsoleApp.Infrastructure;
using FlightsSuggest.ConsoleApp.Timelines;

namespace FlightsSuggest.ConsoleApp.Notifications
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

        public void Notify()
        {
            foreach (var subscriber in subscribers)
            {
                foreach (var notificationSender in senders.Where(s => s.CanSend(subscriber)))
                {
                    foreach (var timeline in timelines)
                    {
                        var offsetId = $"{subscriber.Id}_{timeline.Name}";
                        var offset = offsetStorage.Find(offsetId);
                        if (offset == null)
                        {
                            var latestOffset = timeline.LatestOffset;
                            if (latestOffset.HasValue)
                            {
                                offsetStorage.Write(offsetId, latestOffset.Value);
                            }
                            continue;
                        }

                        foreach (var flightNews in timeline.ReadNews(offset.Value))
                        {
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