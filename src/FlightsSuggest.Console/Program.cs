using FlightsSuggest.ConsoleApp.Infrastructure;
using FlightsSuggest.ConsoleApp.Infrastructure.Vkontakte;
using FlightsSuggest.ConsoleApp.Notifications;
using FlightsSuggest.ConsoleApp.Timelines;

namespace FlightsSuggest.ConsoleApp
{
    static class Program
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

            var notificationSenders = new [] { new ConsoleNotificationSender(), };
            var subscriber = new Subscriber("nick", null, false, new [] {new TermNotificationTrigger("Грец"), });
            var notifier = new Notifier(notificationSenders, new [] { subscriber, }, new [] {vkontakteTimeline}, new FileOffsetStorage());

            notifier.Notify();
        }
    }
}