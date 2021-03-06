﻿using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public class Subscriber
    {
        public Subscriber(
            string id,
            string telegramUsername,
            long? telegramChatId,
            int? telegramUserId,
            bool sendTelegramMessages,
            INotificationTrigger notificationTrigger
            )
        {
            Id = id;
            NotificationTrigger = notificationTrigger;
            TelegramUsername = telegramUsername;
            SendTelegramMessages = sendTelegramMessages;
            TelegramChatId = telegramChatId;
            TelegramUserId = telegramUserId;
        }


        public string Id { get; }
        public string TelegramUsername { get; }
        public bool SendTelegramMessages { get; }
        public long? TelegramChatId { get; }
        public int? TelegramUserId { get; }
        public INotificationTrigger NotificationTrigger { get; }

        public bool ShouldNotify(FlightNews flightNews)
        {
            return NotificationTrigger.ShouldNotify(flightNews);
        }
    }
}