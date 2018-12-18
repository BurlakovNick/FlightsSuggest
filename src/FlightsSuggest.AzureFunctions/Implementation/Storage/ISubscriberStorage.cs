using System.Threading.Tasks;
using FlightsSuggest.Core.Notifications;

namespace FlightsSuggest.AzureFunctions.Implementation.Storage
{
    public interface ISubscriberStorage
    {
        Task<Subscriber> CreateAsync(string telegramUsername);
        Task<Subscriber> UpdateTelegramChatIdAsync(string subscriberId, long telegramChatId);
        Task<Subscriber[]> SelectAllAsync();
        Task<Subscriber> UpdateNotificationTriggerAsync(string subscriberId, INotificationTrigger trigger);
    }
}