using FlightsSuggest.AzureFunctions.Implementation.Storage;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Infrastructure.Vkontakte;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;
using FlightsSuggest.Core.Timelines;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.AzureFunctions.Implementation.Container
{
    public class CoreAppModule : IContainerModule
    {
        private readonly ExecutionContext context;

        public CoreAppModule(ExecutionContext context)
        {
            this.context = context;
        }

        public void Load(IServiceCollection services)
        {
            services.AddSingleton(_ => Core.Configuration.ConfigurationProvider.Provide(context.FunctionAppDirectory));

            services.AddSingleton<IOffsetStorage, AzureTableOffsetStorage>();
            services.AddSingleton<ISubscriberStorage, SubscriberStorage>();
            services.AddSingleton<IFlightNewsStorage, AzureTableFlightNewsStorage>();

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
            services.AddSingleton<FlightNotifier, FlightNotifier>();
        }
    }
}