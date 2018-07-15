using System.Threading.Tasks;

namespace FlightsSuggest.ConsoleApp.Timelines
{
    public interface IFlightNewsStorage
    {
        Task WriteAsync(FlightNews flight);
        Task<FlightNews[]> SelectAsync(long offset, int count, string source);
        Task<long?> FindLatestOffsetAsync(string source);
    }
}