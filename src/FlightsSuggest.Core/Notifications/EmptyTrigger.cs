using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public class EmptyTrigger : INotificationTrigger
    {
        public bool ShouldNotify(FlightNews flightNews) => true;

        public string Serialize() => string.Empty;
    }
}