using System.Threading.Tasks;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;
using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.ConsoleApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = Container.Container.Build();
            foreach (var timeline in serviceProvider.GetServices<ITimeline>())
            {
                await timeline.ActualizeAsync();
            }

            var subscriber = new Subscriber("nick", null, 45921723, null, false, new TermNotificationTrigger("Грец"));

            await serviceProvider.GetRequiredService<INotifier>().NotifyAsync(subscriber);
        }
    }
}