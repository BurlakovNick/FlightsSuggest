using System.Threading.Tasks;

namespace FlightsSuggest.Admin.Infrastructure
{
    public interface IFlightsFunctions
    {
        Task<SubscriberDto[]> SelectAsync();
    }
}