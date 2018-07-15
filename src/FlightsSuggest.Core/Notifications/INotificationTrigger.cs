using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public interface INotificationTrigger
    {
        bool ShouldNotify(FlightNews flightNews);
    }
}