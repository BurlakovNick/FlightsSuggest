using System.IO;
using System.Linq;

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

        public void Write(string id, long offset)
        {
            File.WriteAllLines($"offsets/{id}", new [] {offset.ToString()});
        }

        public long? Find(string id)
        {
            if (!File.Exists($"offsets/{id}"))
            {
                return null;
            }

            var line = File.ReadAllLines($"offsets/{id}").First();
            return long.Parse(line);
        }
    }
}