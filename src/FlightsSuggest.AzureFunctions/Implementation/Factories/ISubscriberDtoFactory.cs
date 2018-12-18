using FlightsSuggest.Core.Notifications;
using FlightsSuggest.Dto;

namespace FlightsSuggest.AzureFunctions.Implementation.Factories
{
    public interface ISubscriberDtoFactory
    {
        SubscriberDto Create(Subscriber subscriber);
        SubscriberDto[] Create(Subscriber[] subscribers);
    }
}