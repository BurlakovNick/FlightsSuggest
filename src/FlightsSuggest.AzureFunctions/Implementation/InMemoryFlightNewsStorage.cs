using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class InMemoryFlightNewsStorage : IFlightNewsStorage
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, FlightNews>> storage;

        public InMemoryFlightNewsStorage()
        {
            storage = new ConcurrentDictionary<string, ConcurrentDictionary<string, FlightNews>>();
        }

        public Task WriteAsync(FlightNews flight)
        {
            GetStorage(flight.Source).AddOrUpdate(flight.Id, _ => flight, (_, __) => flight);
            return Task.CompletedTask;
        }

        public Task<FlightNews[]> SelectAsync(long offset, int count, string source)
        {
            var result = GetStorage(source)
                .Select(x => x.Value)
                .OrderBy(x => x.Offset)
                .Where(x => x.Offset > offset)
                .Take(count)
                .ToArray();
            return Task.FromResult(result);
        }

        public Task<long?> FindLatestOffsetAsync(string source)
        {
            var dictionary = GetStorage(source);
            var result = !dictionary.IsEmpty ? dictionary.Max(x => x.Value.Offset) : (long?) null;
            return Task.FromResult(result);
        }

        private ConcurrentDictionary<string, FlightNews> GetStorage(string sourceName)
        {
            return storage.GetOrAdd(sourceName, _ => new ConcurrentDictionary<string, FlightNews>());
        }
    }
}