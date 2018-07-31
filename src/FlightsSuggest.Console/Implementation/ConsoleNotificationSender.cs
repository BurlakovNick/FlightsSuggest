using System;
using System.Collections.Generic;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;
using Newtonsoft.Json;

namespace FlightsSuggest.ConsoleApp.Implementation
{
    public class ConsoleNotificationSender : INotificationSender
    {
        private readonly List<FlightNews> sended;

        public ConsoleNotificationSender()
        {
            sended = new List<FlightNews>();
        }

        public bool CanSend(Subscriber subscriber)
        {
            return true;
        }

        public void SendTo(Subscriber subscriber, FlightNews flightNews)
        {
            Console.WriteLine($"Message for {subscriber.Id}");
            Console.WriteLine($"{JsonConvert.SerializeObject(flightNews, Formatting.Indented)}");
        }

        public FlightNews[] Sended => sended.ToArray();
    }
}