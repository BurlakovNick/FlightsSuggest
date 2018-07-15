using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public interface INotificationSender
    {
        bool CanSend(Subscriber subscriber);
        void SendTo(Subscriber subscriber, FlightNews flightNews);
    }
}