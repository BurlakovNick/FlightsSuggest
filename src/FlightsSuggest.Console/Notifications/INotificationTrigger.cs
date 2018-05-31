using FlightsSuggest.ConsoleApp.Timelines;

namespace FlightsSuggest.ConsoleApp.Notifications
{
    public interface INotificationTrigger
    {
        bool ShouldNotify(FlightNews flightNews);
    }
}