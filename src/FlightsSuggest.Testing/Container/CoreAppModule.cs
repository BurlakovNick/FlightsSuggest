using FlightsSuggest.AzureFunctions.Implementation;
using FlightsSuggest.AzureFunctions.Implementation.Storage;
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

            services.AddSingleton<INotificationSender, TelegramNotificationSender>();

            services.AddSingleton<ITelegramClient, TelegramClient>();
            services.AddSingleton<IVkontakteClient, VkontakteClient>();

            services.AddSingleton<IFlightNewsFactory, FlightNewsFactory>();

            services.AddSingleton<ITimeline>(serviceProvider => new VkontakteTimeline(
                "vandroukiru",
                serviceProvider.GetService<IOffsetStorage>(),
                serviceProvider.GetService<IFlightNewsStorage>(),
                serviceProvider.GetService<IVkontakteClient>(),
                serviceProvider.GetService<IFlightNewsFactory>()
            ));

            services.AddSingleton<INotifier, Notifier>();
        }
    }
}