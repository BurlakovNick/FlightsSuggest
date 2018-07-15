using System.Threading.Tasks;

namespace FlightsSuggest.Core.Infrastructure
{
    public interface IAsyncEnumerator<T>
    {
        Task<(bool hasNext, T nextItem)> MoveNextAsync();
    }
}