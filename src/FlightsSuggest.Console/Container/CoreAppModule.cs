﻿using FlightsSuggest.ConsoleApp.Implementation;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Infrastructure.Vkontakte;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;
using FlightsSuggest.Core.Timelines;
using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.ConsoleApp.Container
{
    public class CoreAppModule : IContainerModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddSingleton(_ => Core.Configuration.ConfigurationProvider.ProvideLocal());

            services.AddSingleton<IOffsetStorage, FileOffsetStorage>();
            services.AddSingleton<IFlightNewsStorage, FileFlightNewsStorage>();

            services.AddSingleton(_ =>
                new INotificationSender[]
                {
                    new ConsoleNotificationSender(),
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