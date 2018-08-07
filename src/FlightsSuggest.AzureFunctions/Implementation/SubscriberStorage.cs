using System;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Notifications;
using Microsoft.WindowsAzure.Storage.Table;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class SubscriberStorage : ISubscriberStorage
    {
        private const string GlobalPartitionKey = "subscribers";
        private static readonly INotificationTrigger[] FakeTriggers = new[] {new TermNotificationTrigger("Греци"),};

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
            var subscriber = new Subscriber(id, telegramUsername, null, false, FakeTriggers);
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

        private static Subscriber Convert(SubscriberDbo x)
        {
            return new Subscriber(x.Id, x.TelegramUsername, x.TelegramChatId, x.SendTelegramMessages, FakeTriggers);
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
            }

            public string Id { get; set;  }
            public string TelegramUsername { get; set; }
            public bool SendTelegramMessages { get; set; }
            public long? TelegramChatId { get; set; }
        }
    }
}