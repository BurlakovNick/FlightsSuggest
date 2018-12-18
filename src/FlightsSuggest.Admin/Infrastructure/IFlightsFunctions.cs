using System.Threading.Tasks;
using FlightsSuggest.Dto;

namespace FlightsSuggest.Admin.Infrastructure
{
    public interface IFlightsFunctions
    {
        Task<SubscriberDto[]> SelectAsync();
    }
}