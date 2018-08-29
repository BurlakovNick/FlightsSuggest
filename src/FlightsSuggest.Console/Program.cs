using System.Threading.Tasks;
using FlightsSuggest.ConsoleApp.Implementation;
using FlightsSuggest.Core.Infrastructure.Vkontakte;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.ConsoleApp
{
    static class Program
    {
        static async Task Main(string[] args)
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

            await vkontakteTimeline.ActualizeAsync();

            var notificationSenders = new [] { new ConsoleNotificationSender(), };
            var subscriber = new Subscriber("nick", null, 45921723, false, new TermNotificationTrigger("Грец"));
            var notifier = new Notifier(notificationSenders, new [] { subscriber, }, new [] {vkontakteTimeline}, new FileOffsetStorage());

            await notifier.NotifyAsync();
        }
    }
}