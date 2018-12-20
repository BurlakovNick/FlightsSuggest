using FlightsSuggest.AzureFunctions.Implementation;
using FlightsSuggest.AzureFunctions.Implementation.Storage;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Infrastructure.Vkontakte;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;
using FlightsSuggest.Core.Timelines;
using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.Testing.Container
{
    public class CoreAppModule : IContainerModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddSingleton(_ => Core.Configuration.ConfigurationProvider.ProvideLocal());

            services.AddSingleton<IOffsetStorage, AzureTableOffsetStorage>();
            services.AddSingleton<IFlightNewsStorage, AzureTableFlightNewsStorage>();
            services.AddSingleton<ISubscriberStorage, SubscriberStorage>();

            services.AddSingleton(serviceProvider =>
                new INotificationSender[]
                {
                    new TelegramNotificationSender(serviceProvider.GetService<IFlightsConfiguration>())
                });

            services.AddSingleton<ITelegramClient, TelegramClient>();
            services.AddSingleton<IVkontakteClient, VkontakteClient>();

            services.AddSingleton<IFlightNewsFactory, FlightNewsFactory>();

            services.AddSingleton(serviceProvider =>
                new ITimeline[]
                {
                    new VkontakteTimeline(
                        "vandroukiru",
                        serviceProvider.GetRequiredService<IOffsetStorage>(),
                        serviceProvider.GetRequiredService<IFlightNewsStorage>(),
                        serviceProvider.GetRequiredService<IVkontakteClient>(),
                        serviceProvider.GetRequiredService<IFlightNewsFactory>()
                    )
                });

            services.AddSingleton<INotifier, Notifier>();
        }
    }
}