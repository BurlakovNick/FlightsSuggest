using System.Threading.Tasks;
using FlightsSuggest.Core.Notifications;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public interface ISubscriberStorage
    {
        Task<Subscriber> CreateAsync(string telegramUsername);
        Task<Subscriber> UpdateTelegramChatIdAsync(string subscriberId, long telegramChatId);
        Task<Subscriber[]> SelectAllAsync();
    }
}