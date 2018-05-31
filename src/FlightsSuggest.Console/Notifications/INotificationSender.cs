using FlightsSuggest.ConsoleApp.Timelines;

namespace FlightsSuggest.ConsoleApp.Notifications
{
    public interface INotificationSender
    {
        bool CanSend(Subscriber subscriber);
        void SendTo(Subscriber subscriber, FlightNews flightNews);
    }
}