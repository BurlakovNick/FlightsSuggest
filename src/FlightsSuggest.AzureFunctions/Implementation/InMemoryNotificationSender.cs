using System.Collections.Generic;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class InMemoryNotificationSender : INotificationSender
    {
        private readonly List<FlightNews> sended;

        public InMemoryNotificationSender()
        {
            sended = new List<FlightNews>();
        }

        public bool CanSend(Subscriber subscriber) => true;

        public void SendTo(Subscriber subscriber, FlightNews flightNews)
        {
            sended.Add(flightNews);
        }

        public FlightNews[] Sended => sended.ToArray();
    }
}