using System.Threading.Tasks;

namespace FlightsSuggest.Core.Timelines
{
    public interface IFlightNewsStorage
    {
        Task WriteAsync(FlightNews flight);
        Task<FlightNews[]> SelectAsync(long offset, int count, string source);
        Task<long?> FindLatestOffsetAsync(string source);
        Task DeleteAsync(FlightNews flight);
    }
}