using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Timelines;
using Newtonsoft.Json;

namespace FlightsSuggest.ConsoleApp.Implementation
{
    public class FileFlightNewsStorage : IFlightNewsStorage
    {
        public FileFlightNewsStorage()
        {
            if (!Directory.Exists("flights"))
            {
                Directory.CreateDirectory("flights");
            }
        }

        public async Task WriteAsync(FlightNews flight)
        {
            var filename = GetFilename(flight.Source);
            if (!File.Exists(filename))
            {
                await File.WriteAllLinesAsync(filename, new [] {JsonConvert.SerializeObject(flight)});
                return;
            }

            var lines = await File.ReadAllLinesAsync(filename);
            var flights = lines.Select(JsonConvert.DeserializeObject<FlightNews>).ToArray();
            if (flights.All(f => f.Id != flight.Id))
            {
                await File.WriteAllLinesAsync(filename, flights.Concat(new [] {flight}).Select(JsonConvert.SerializeObject).ToArray());
            }
        }

        public async Task<FlightNews[]> SelectAsync(long offset, int count, string source)
        {
            var filename = GetFilename(source);
            if (!File.Exists(filename))
            {
                return new FlightNews[0];
            }

            var lines = await File.ReadAllLinesAsync(filename);
            var flights = lines.Select(JsonConvert.DeserializeObject<FlightNews>).ToArray();
            return flights.OrderBy(x => x.Offset).Where(x => x.Offset > offset).Take(count).ToArray();
        }

        public async Task<long?> FindLatestOffsetAsync(string source)
        {
            var filename = GetFilename(source);
            if (!File.Exists(filename))
            {
                return null;
            }

            var lines = await File.ReadAllLinesAsync(filename);
            var flights = lines.Select(JsonConvert.DeserializeObject<FlightNews>).ToArray();
            return flights.Max(x => x.Offset);
        }

        private static string GetFilename(string source)
        {
            return $"flights/{source}";
        }
    }
}