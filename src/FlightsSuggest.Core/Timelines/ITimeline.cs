using System.Threading.Tasks;
using FlightsSuggest.Core.Infrastructure;

namespace FlightsSuggest.Core.Timelines
{
    public interface ITimeline
    {
        Task ActualizeAsync();
        IAsyncEnumerator<FlightNews> GetNewsEnumerator(long offset);
        Task<long?> GetLatestOffsetAsync();
        string Name { get; }
    }
}