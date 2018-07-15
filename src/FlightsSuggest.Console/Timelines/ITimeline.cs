using System.Threading.Tasks;
using FlightsSuggest.ConsoleApp.Infrastructure;

namespace FlightsSuggest.ConsoleApp.Timelines
{
    public interface ITimeline
    {
        Task ActualizeAsync();
        IAsyncEnumerator<FlightNews> GetNewsEnumerator(long offset);
        Task<long?> GetLatestOffsetAsync();
        string Name { get; }
    }
}