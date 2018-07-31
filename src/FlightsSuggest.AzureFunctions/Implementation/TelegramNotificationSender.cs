using System.Collections.Generic;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Core.Timelines;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class TelegramNotificationSender : INotificationSender
    {
        private readonly TelegramBotClient botClient;
        private readonly List<FlightNews> sended;

        public TelegramNotificationSender(
            IFlightsConfiguration configuration
        )
        {
            sended = new List<FlightNews>();
            botClient = new TelegramBotClient(configuration.TelegramBotToken);
        }

        public bool CanSend(Subscriber subscriber) => subscriber.SendTelegramMessages && subscriber.TelegramChatId.HasValue;

        public void SendTo(Subscriber subscriber, FlightNews flightNews)
        {
            if (subscriber.TelegramChatId == null)
            {
                return;
            }

            botClient.SendTextMessageAsync(new ChatId(subscriber.TelegramChatId.Value), flightNews.Text);
            sended.Add(flightNews);
        }

        public FlightNews[] Sended => sended.ToArray();
    }
}