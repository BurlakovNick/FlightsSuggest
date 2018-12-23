using System.Threading.Tasks;
using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Dto;

namespace FlightsSuggest.AzureFunctions.Implementation.Factories
{
    public interface ISubscriberDtoFactory
    {
        Task<SubscriberDto[]> CreateAsync(Subscriber[] subscribers);
        Task<SubscriberDto> CreateAsync(Subscriber subscriber);
    }
}