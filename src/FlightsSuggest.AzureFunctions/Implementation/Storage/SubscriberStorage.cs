using System;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Notifications;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace FlightsSuggest.AzureFunctions.Implementation.Storage
{
    public class SubscriberStorage : ISubscriberStorage
    {
        private const string GlobalPartitionKey = "subscribers";

        private readonly CloudTable subscribersTable;

        public SubscriberStorage(IFlightsConfiguration flightsConfiguration)
        {
            subscribersTable = flightsConfiguration.GetAzureTableAsync("Subscribers").Result;
        }

        public async Task<Subscriber> CreateAsync(string telegramUsername)
        {
            var existSubscriber = await FindUserAsync(telegramUsername);
            if (existSubscriber != null)
            {
                return existSubscriber;
            }

            var id = Guid.NewGuid().ToString();
            var subscriber = new Subscriber(id, telegramUsername, null, false, new EmptyTrigger());
            await subscribersTable.WriteAsync(new SubscriberDbo(subscriber));
            return subscriber;
        }

        public async Task<Subscriber> UpdateTelegramChatIdAsync(string subscriberId, long telegramChatId)
        {
            var subscriber = await subscribersTable.FindAsync<SubscriberDbo>(GlobalPartitionKey, subscriberId);
            if (subscriber == null)
            {
                return null;
            }

            subscriber.TelegramChatId = telegramChatId;
            subscriber.SendTelegramMessages = true;

            await subscribersTable.WriteAsync(subscriber);

            return Convert(subscriber);
        }

        public async Task<Subscriber> UpdateNotificationTriggerAsync(string subscriberId, INotificationTrigger trigger)
        {
            var subscriber = await subscribersTable.FindAsync<SubscriberDbo>(GlobalPartitionKey, subscriberId);
            if (subscriber == null)
            {
                return null;
            }

            subscriber.SearchSettings = trigger.Serialize();

            await subscribersTable.WriteAsync(subscriber);

            return Convert(subscriber);
        }

        public async Task<Subscriber[]> SelectAllAsync()
        {
            var dbos = await subscribersTable.SelectAsync<SubscriberDbo>(GlobalPartitionKey, String.Empty, 1000);
            return dbos
                .Select(Convert)
                .ToArray();
        }

        private async Task<Subscriber> FindUserAsync(string telegramUsername)
        {
            var subscribers = await SelectAllAsync();
            return subscribers.FirstOrDefault(x => x.TelegramUsername == telegramUsername);
        }

        private static Subscriber Convert(SubscriberDbo dbo)
        {
            var trigger = NotificationTriggers.BuildFromText(dbo.SearchSettings);
            if (!trigger.success)
            {
                throw new InvalidOperationException($"Can't parse setting for subscriber {JsonConvert.SerializeObject(dbo)}");
            }
            return new Subscriber(dbo.Id, dbo.TelegramUsername, dbo.TelegramChatId, dbo.SendTelegramMessages, trigger.result);
        }

        class SubscriberDbo : TableEntity
        {
            public SubscriberDbo()
            {
            }

            public SubscriberDbo(Subscriber subscriber)
            {
                PartitionKey = GlobalPartitionKey;
                RowKey = subscriber.Id;

                Id = subscriber.Id;
                TelegramUsername = subscriber.TelegramUsername;
                SendTelegramMessages = subscriber.SendTelegramMessages;
                TelegramChatId = subscriber.TelegramChatId;
                SearchSettings = subscriber.NotificationTrigger.Serialize();
            }

            public string Id { get; set;  }
            public string TelegramUsername { get; set; }
            public bool SendTelegramMessages { get; set; }
            public long? TelegramChatId { get; set; }
            public string SearchSettings { get; set; }
        }
    }
}