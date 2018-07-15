using System.Collections.Concurrent;
using System.Threading.Tasks;
using FlightsSuggest.Core.Infrastructure;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class InMemoryOffsetStorage : IOffsetStorage
    {
        private readonly ConcurrentDictionary<string, long> storage;

        public InMemoryOffsetStorage()
        {
            storage = new ConcurrentDictionary<string, long>();
        }

        public Task WriteAsync(string id, long offset)
        {
            storage.AddOrUpdate(id, _ => offset, (_, __) => offset);
            return Task.CompletedTask;
        }

        public Task<long?> FindAsync(string id)
        {
            var offset = storage.TryGetValue(id, out var result)
                ? result
                : (long?) null;
            return Task.FromResult(offset);
        }
    }
}