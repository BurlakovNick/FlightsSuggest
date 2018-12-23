using System.Threading.Tasks;
using FlightsSuggest.Core.Notifications;

namespace FlightsSuggest.AzureFunctions.Implementation.Storage
{
    public interface ISubscriberStorage
    {
        Task<Subscriber> CreateAsync(string telegramUsername, long telegramChatId, int telegramUserId);
        Task<Subscriber> UpdateSubscriberAsync(string subscriberId, long telegramChatId, int telegramUserId);
        Task<Subscriber[]> SelectAllAsync();
        Task<Subscriber> UpdateNotificationTriggerAsync(string subscriberId, INotificationTrigger trigger);
    }
}