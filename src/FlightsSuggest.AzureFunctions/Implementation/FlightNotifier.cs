using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation.Commands;
using FlightsSuggest.AzureFunctions.Implementation.Factories;
using FlightsSuggest.AzureFunctions.Implementation.Storage;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;
using FlightsSuggest.Core.Timelines;
using FlightsSuggest.Dto;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class FlightNotifier
    {
        private readonly ITelegramClient telegramClient;
        private readonly ITimeline[] timelines;
        private readonly INotifier notifier;
        private readonly INotificationSender[] notificationSenders;
        private readonly ISubscriberStorage subscriberStorage;
        private readonly ISubscriberDtoFactory subscriberDtoFactory;
        private readonly ICommandParser commandParser;

        public FlightNotifier(
            ITelegramClient telegramClient,
            ITimeline[] timelines,
            INotifier notifier,
            INotificationSender[] notificationSenders,
            ISubscriberStorage subscriberStorage,
            ISubscriberDtoFactory subscriberDtoFactory,
            ICommandParser commandParser
            )
        {
            this.telegramClient = telegramClient;
            this.timelines = timelines;
            this.notifier = notifier;
            this.notificationSenders = notificationSenders;
            this.subscriberStorage = subscriberStorage;
            this.subscriberDtoFactory = subscriberDtoFactory;
            this.commandParser = commandParser;
        }

        public FlightNews[] Sended { get; private set; }

        public async Task NotifyAsync()
        {
            foreach (var timeline in timelines)
            {
                await timeline.ActualizeAsync();
            }

            var subscribers = await subscriberStorage.SelectAllAsync();
            await notifier.NotifyAsync(subscribers);

            Sended = notificationSenders.SelectMany(x => x.Sended).ToArray();
        }

        public Task RewindSubscriberOffsetAsync(string subscriberId, string timelineName, long offset)
        {
            return notifier.RewindOffsetAsync(subscriberId, timelineName, offset);
        }

        public async Task RewindVkOffsetAsync(string vkGroup, long offset)
        {
            var timeline = timelines.OfType<VkontakteTimeline>().FirstOrDefault(x => x.VkGroupName == vkGroup);
            if (timeline == null)
            {
                return;
            }

            await timeline.WriteOffsetAsync(offset);
        }

        public async Task<SubscriberDto[]> SelectSubscribersAsync()
        {
            var subscribers = await subscriberStorage.SelectAllAsync();
            return await subscriberDtoFactory.CreateAsync(subscribers);
        }

        public Task<(string timelineName, DateTime? offset)[]> SelectOffsetsAsync(string subscriberId)
        {
            return notifier.SelectOffsetsAsync(subscriberId);
        }

        public async Task<(string vkGroupName, DateTime? offset)[]> SelectVkOffsetsAsync()
        {
            var result = new List<(string, DateTime?)>();
            foreach (var timeline in timelines.OfType<VkontakteTimeline>())
            {
                var offset = await timeline.GetLatestOffsetAsync();
                if (offset.HasValue)
                {
                    result.Add((timeline.VkGroupName, new DateTime(offset.Value)));
                }
                else
                {
                    result.Add((timeline.VkGroupName, null));
                }
            }

            return result.ToArray();
        }

        public async Task ProcessTelegramUpdateAsync(TelegramUpdate update, ILogger log)
        {
            log.LogInformation($"Received update: {JsonConvert.SerializeObject(update)}");

            var telegramUsername = update.Username;
            var chatId = update.ChatId;
            var userId = update.UserId;

            var subscriber = await GetSubscriberAsync(telegramUsername, chatId, userId);

            var parseResult = commandParser.Parse(update.Text);
            if (!parseResult.IsSuccess)
            {
                await telegramClient.SendMessageAsync(chatId, $"Не могу понять тебя. Ошибка: {parseResult.ErrorMessage}");
                return;
            }

            var command = parseResult.Command;
            await command.ExecuteAsync(subscriber, chatId);
        }

        private async Task<Subscriber> GetSubscriberAsync(string telegramUsername, long chatId, int userId)
        {
            var subscribers = (await subscriberStorage.SelectAllAsync()).ToDictionary(x => x.TelegramUsername);
            subscribers.TryGetValue(telegramUsername, out var subscriber);

            if (subscriber == null)
            {
                subscriber = await subscriberStorage.CreateAsync(telegramUsername, chatId, userId);
                await telegramClient.SendMessageAsync(chatId, "Привет, подписчик!");
            }

            if (!subscriber.TelegramChatId.HasValue || subscriber.TelegramChatId == 0 ||
                !subscriber.TelegramUserId.HasValue || subscriber.TelegramUserId == 0)
            {
                subscriber = await subscriberStorage.UpdateSubscriberAsync(subscriber.Id, chatId, userId);
            }

            return subscriber;
        }
    }
}