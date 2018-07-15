using System.Threading.Tasks;

namespace FlightsSuggest.ConsoleApp.Infrastructure
{
    public interface IOffsetStorage
    {
        Task WriteAsync(string id, long offset);
        Task<long?> FindAsync(string id);
    }
}