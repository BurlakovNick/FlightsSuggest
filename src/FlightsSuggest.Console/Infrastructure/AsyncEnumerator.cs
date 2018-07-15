using System.Threading.Tasks;

namespace FlightsSuggest.ConsoleApp.Infrastructure
{
    public interface IAsyncEnumerator<T>
    {
        Task<(bool hasNext, T nextItem)> MoveNextAsync();
    }
}