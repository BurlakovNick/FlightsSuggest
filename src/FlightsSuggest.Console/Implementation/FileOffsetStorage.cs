using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Infrastructure;

namespace FlightsSuggest.ConsoleApp.Implementation
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
            var filename = GetFilename(id);
            return File.WriteAllLinesAsync(filename, new [] {offset.ToString()});
        }

        public async Task<long?> FindAsync(string id)
        {
            var filename = GetFilename(id);
            if (!File.Exists(filename))
            {
                return null;
            }

            var line = (await File.ReadAllLinesAsync(filename)).First();
            return long.Parse(line);
        }

        public Task DeleteAsync(string id)
        {
            var filename = GetFilename(id);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            return Task.CompletedTask;
        }

        private static string GetFilename(string id) => $"offsets/{id}";
    }
}