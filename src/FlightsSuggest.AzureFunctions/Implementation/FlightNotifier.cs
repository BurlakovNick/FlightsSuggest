using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation.Storage;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Telegram;
using FlightsSuggest.Core.Timelines;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class FlightNotifier
    {
        private readonly IFlightsConfiguration configuration;
        private readonly ITelegramClient telegramClient;
        private readonly ITimeline[] timelines;
        private readonly INotifier notifier;
        private readonly INotificationSender telegramNotificationSender;
        private readonly ISubscriberStorage subscriberStorage;

        public FlightNotifier(
            IFlightsConfiguration configuration,
            ITelegramClient telegramClient,
            ITimeline[] timelines,
            INotifier notifier,
            INotificationSender telegramNotificationSender,
            ISubscriberStorage subscriberStorage)
        {
            this.configuration = configuration;
            this.telegramClient = telegramClient;
            this.timelines = timelines;
            this.notifier = notifier;
            this.telegramNotificationSender = telegramNotificationSender;
            this.subscriberStorage = subscriberStorage;
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

            Sended = telegramNotificationSender.Sended;
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

        public Task<Subscriber[]> SelectSubscribersAsync()
        {
            return subscriberStorage.SelectAllAsync();
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

        public Task CreateSubscriberAsync(string telegramUsername)
        {
            return subscriberStorage.CreateAsync(telegramUsername);
        }

        public async Task ProcessTelegramUpdateAsync(TelegramUpdate update, ILogger log)
        {
            log.LogInformation($"Received update: {JsonConvert.SerializeObject(update)}");

            var message = update.Text.ToLower();
            var telegramUsername = update.Username;
            var chatId = update.ChatId;

            var subscribers = (await subscriberStorage.SelectAllAsync()).ToDictionary(x => x.TelegramUsername);
            subscribers.TryGetValue(telegramUsername, out var subscriber);

            if (message.Contains(configuration.TelegramMagicWords))
            {
                log.LogInformation("Seen magic words, creating new subscriber");

                if (subscriber?.TelegramChatId != null)
                {
                    log.LogInformation("Subscriber already created");
                    return;
                }

                if (subscriber == null)
                {
                    subscriber = await subscriberStorage.CreateAsync(telegramUsername);
                }

                await subscriberStorage.UpdateTelegramChatIdAsync(subscriber.Id, chatId);
                await telegramClient.SendMessageAsync(update.ChatId, "Привет, подписчик!");
                return;
            }

            if (subscriber == null)
            {
                return;
            }

            if (message.StartsWith(configuration.TelegramSearchSettingWords))
            {
                log.LogInformation("Seen search setting words, lets set up search");

                var setting = message.Remove(0, configuration.TelegramSearchSettingWords.Length);
                var trigger = NotificationTriggers.BuildFromText(setting);

                if (!trigger.success)
                {
                    log.LogError($"Can't parse trigger expression. Error: {trigger.message}");
                    await telegramClient.SendMessageAsync(chatId, $"Что-то не так с настройкой. Ошибка: {trigger.message}");
                    return;
                }

                await subscriberStorage.UpdateNotificationTriggerAsync(subscriber.Id, trigger.result);
                await telegramClient.SendMessageAsync(chatId, "Принято, босс! Теперь бот будет присылать только новости с такими словами");

                return;
            }

            if (message == configuration.TelegramSearchSettingRequestWords)
            {
                await telegramClient.SendMessageAsync(chatId,
                    "Чтобы настроить поиск, нужно послать боту сообщение в таком формате:");

                await telegramClient.SendMessageAsync(chatId,
                    $"{configuration.TelegramSearchSettingWords} [Дублин, Майорка, Вена]");

                await telegramClient.SendMessageAsync(chatId,
                    "Бот будет искать новости, в тексте который есть слова 'Дублин', 'Майорка' или 'Вена'");

                await telegramClient.SendMessageAsync(chatId,
                    "Если хочешь, чтобы обязательно встретилось несколько слов, можешь обернуть их в круглые скобки:");

                await telegramClient.SendMessageAsync(chatId,
                    $"{configuration.TelegramSearchSettingWords} (Тайланд, дешево)");

                await telegramClient.SendMessageAsync(chatId,
                    "Условия можно комбинировать как тебе захочется! Например, если хочешь полететь в Тайланд дешево или в Дублин дорого, сделай так:");

                await telegramClient.SendMessageAsync(chatId,
                    $"{configuration.TelegramSearchSettingWords} [(Тайланд, дешево), (Дублин, дорого)]");

                return;
            }

            if (message == configuration.TelegramLastNewsRequestFormat)
            {
                var replyKeyboard = new[] {1, 3, 7, 14}
                    .Aggregate(new ReplyKeyboardBuilder(), (builder, x) =>
                    {
                        var buttonText = PatternParser.ReplacePatternWithInt(configuration.TelegramLastNewsFormat, x);
                        return builder.AddRow(new ReplyKeyboardButton(buttonText));
                    })
                    .AddRow(new ReplyKeyboardButton("Обратно!"));

                await telegramClient.SendMessageAsync(
                    chatId,
                    "За какой период будем искать?",
                    replyKeyboard);
                return;
            }

            var lastNewsParseResult = PatternParser.ParseExpressionWithInt(configuration.TelegramLastNewsFormat, message);
            if (lastNewsParseResult.success)
            {
                var newDate = DateTime.UtcNow.AddDays(-lastNewsParseResult.result.Value);
                var newOffset = newDate.Ticks;

                foreach (var timeline in timelines)
                {
                    log.LogInformation($"Rewinding {telegramUsername} offset in {timeline.Name} timeline to {newDate}");
                    await RewindSubscriberOffsetAsync(subscriber.Id, timeline.Name, newOffset);
                    await notifier.NotifyAsync(subscriber);
                    await telegramClient.SendMessageAsync(chatId, "Сделано, хозяин!");
                }

                return;
            }

            var menuKeyboard = new ReplyKeyboardBuilder()
                .AddRow(new ReplyKeyboardButton(configuration.TelegramSearchSettingRequestWords))
                .AddRow(new ReplyKeyboardButton(configuration.TelegramLastNewsRequestFormat));

            await telegramClient.SendMessageAsync(
                chatId,
                "Этот бот поможет тебе найти дешевые билеты в интернетах. Что сделать для тебя, дружище?",
                menuKeyboard);
        }
    }
}