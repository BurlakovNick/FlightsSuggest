using System;
using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public class TelegramNotificationSender : INotificationSender
    {
        public bool CanSend(Subscriber subscriber)
        {
            return subscriber.SendTelegramMessages &&
                   !string.IsNullOrEmpty(subscriber.TelegramUsername);
        }

        public void SendTo(Subscriber subscriber, FlightNews flightNews)
        {
            throw new NotImplementedException();
        }
    }
}