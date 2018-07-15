using System;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;
using Newtonsoft.Json;

namespace FlightsSuggest.ConsoleApp.Implementation
{
    public class ConsoleNotificationSender : INotificationSender
    {
        public bool CanSend(Subscriber subscriber)
        {
            return true;
        }

        public void SendTo(Subscriber subscriber, FlightNews flightNews)
        {
            Console.WriteLine($"Message for {subscriber.Id}");
            Console.WriteLine($"{JsonConvert.SerializeObject(flightNews, Formatting.Indented)}");
        }
    }
}