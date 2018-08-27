using System.Linq;
using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public class ShouldNotificationTrigger : INotificationTrigger
    {
        private readonly INotificationTrigger[] triggers;

        public ShouldNotificationTrigger(
            INotificationTrigger[] triggers
        )
        {
            this.triggers = triggers ?? new INotificationTrigger[0];
        }

        public bool ShouldNotify(FlightNews flightNews)
        {
            return triggers.Any(t => t.ShouldNotify(flightNews));
        }

        public string Serialize()
        {
            return "[" + string.Join(",", triggers.Select(x => x.Serialize())) + "]";
        }
    }
}