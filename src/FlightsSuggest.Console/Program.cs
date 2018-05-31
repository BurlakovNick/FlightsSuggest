using System;
using System.Linq;
using FlightsSuggest.ConsoleApp.Infrastructure;
using FlightsSuggest.ConsoleApp.Infrastructure.Vkontakte;
using FlightsSuggest.ConsoleApp.Notifications;
using FlightsSuggest.ConsoleApp.Timelines;
using Newtonsoft.Json;
using VkNet;
using VkNet.Model.RequestParams;

namespace FlightsSuggest.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var vkApplicationId = (ulong)int.Parse(args[0]);
            var vkAccessToken = args[1];

            var vkontakteTimeline = new VkontakteTimeline(
                "vandroukiru",
                new FileOffsetStorage(),
                new FileFlightNewsStorage(),
                new VkontakteClient(vkApplicationId, vkAccessToken),
                new FlightNewsFactory()
            );

            vkontakteTimeline.Actualize();

            Console.WriteLine(vkontakteTimeline.LatestOffset);
            foreach (var line in vkontakteTimeline.ReadNews(0).Select(JsonConvert.SerializeObject))
            {
                Console.WriteLine(line);
            }
        }
    }
}