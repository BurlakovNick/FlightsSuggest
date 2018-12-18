using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.AzureFunctions.Implementation.Storage;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Infrastructure.Vkontakte;
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
        private readonly VkontakteClient vkontakteClient;
        private VkontakteTimeline vkontakteTimeline;
        private Notifier notifier;
        private INotificationSender telegramNotificationSender;
        private SubscriberStorage subscriberStorage;
        private TelegramBotClient botClient;

        public FlightNotifier(
            IFlightsConfiguration configuration
            )
        {
            this.configuration = configuration;
            vkontakteClient = new VkontakteClient(configuration.VkApplicationId, configuration.VkAccessToken);
            Init();
        }

        public FlightNews[] Sended { get; private set; }

        public async Task NotifyAsync()
        {
            await vkontakteTimeline.ActualizeAsync();
            await notifier.NotifyAsync();
            Sended = telegramNotificationSender.Sended;
        }

        public Task RewindSubscriberOffsetAsync(string subscriberId, string timelineName, long offset)
        {
            return notifier.RewindOffsetAsync(subscriberId, timelineName, offset);
        }

        public async Task RewindVkOffsetAsync(string vkGroup, long offset)
        {
            var timeline = new [] {vkontakteTimeline}.FirstOrDefault(x => x.VkGroupName == vkGroup);
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
            var timelines = new [] { vkontakteTimeline};
            var result = new List<(string, DateTime?)>();
            foreach (var timeline in timelines)
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
                var lastNewsKeyboard = new ReplyKeyboardMarkup
                {
                    Keyboard = new[] {1, 3, 7, 14}
                        .Select(x => new[]
                        {
                            new KeyboardButton(PatternParser.ReplacePatternWithInt(configuration.TelegramLastNewsFormat, x))
                        })
                        .Concat(new[] { new [] {new KeyboardButton("Обратно!")} })
                        .ToArray(),
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(
                    new ChatId(update.Message.Chat.Id),
                    "За какой период будем искать?",
                    replyMarkup: lastNewsKeyboard);
                return;
            }

            var lastNewsParseResult = PatternParser.ParseExpressionWithInt(configuration.TelegramLastNewsFormat, message);
            if (lastNewsParseResult.success)
            {
                var newDate = DateTime.UtcNow.AddDays(-lastNewsParseResult.result.Value);
                var newOffset = newDate.Ticks;
                log.LogInformation($"Rewinding {telegramUsername} offset in vkontakte timeline to {newDate}");
                await RewindSubscriberOffsetAsync(subscriber.Id, vkontakteTimeline.Name, newOffset);
                await notifier.NotifyAsync(subscriber);
                await botClient.SendTextMessageAsync(new ChatId(update.Message.Chat.Id), "Сделано, хозяин!");

                return;
            }

            var menuKeyboard = new ReplyKeyboardMarkup
            {
                Keyboard = new []
                {
                    new []
                    {
                        new KeyboardButton(configuration.TelegramSearchSettingRequestWords),
                        new KeyboardButton(configuration.TelegramLastNewsRequestFormat),
                    }
                },
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                new ChatId(update.Message.Chat.Id),
                "Этот бот поможет тебе найти дешевые билеты в интернетах. Что сделать для тебя, дружище?",
                replyMarkup: menuKeyboard);
        }

        private void Init()
        {
            var offsetStorage = new AzureTableOffsetStorage(configuration);

            vkontakteTimeline = new VkontakteTimeline(
                "vandroukiru",
                offsetStorage,
                new AzureTableFlightNewsStorage(configuration), 
                vkontakteClient,
                new FlightNewsFactory()
            );

            telegramNotificationSender = new TelegramNotificationSender(configuration);
            subscriberStorage = new SubscriberStorage(configuration);
            botClient = new TelegramBotClient(configuration.TelegramBotToken);

            var notificationSenders = new[] { telegramNotificationSender, };
            var subscribers = subscriberStorage.SelectAllAsync().GetAwaiter().GetResult();

            notifier = new Notifier(notificationSenders, subscribers, new ITimeline[] { vkontakteTimeline }, offsetStorage);
        }
    }
}