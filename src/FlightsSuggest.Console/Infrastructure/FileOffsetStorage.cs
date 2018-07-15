using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlightsSuggest.ConsoleApp.Infrastructure
{
    public class FileOffsetStorage : IOffsetStorage
    {
        public FileOffsetStorage()
        {
            if (!Directory.Exists("offsets"))
            {
                Directory.CreateDirectory("offsets");
            }
        }

        public Task WriteAsync(string id, long offset)
        {
            return File.WriteAllLinesAsync($"offsets/{id}", new [] {offset.ToString()});
        }

        public async Task<long?> FindAsync(string id)
        {
            if (!File.Exists($"offsets/{id}"))
            {
                return null;
            }

            var line = (await File.ReadAllLinesAsync($"offsets/{id}")).First();
            return long.Parse(line);
        }
    }
}