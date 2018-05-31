using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FlightsSuggest.ConsoleApp.Timelines
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

        public void Write(FlightNews flight)
        {
            var filename = GetFilename(flight.Source);
            if (!File.Exists(filename))
            {
                File.WriteAllLines(filename, new [] {JsonConvert.SerializeObject(flight)});
                return;
            }

            var lines = File.ReadAllLines(filename);
            var flights = lines.Select(JsonConvert.DeserializeObject<FlightNews>).ToArray();
            if (flights.All(f => f.Id != flight.Id))
            {
                File.WriteAllLines(filename, flights.Concat(new [] {flight}).Select(JsonConvert.SerializeObject).ToArray());
            }
        }

        public FlightNews[] Select(long offset, int count, string source)
        {
            var filename = GetFilename(source);
            if (!File.Exists(filename))
            {
                return new FlightNews[0];
            }

            var lines = File.ReadAllLines(filename);
            var flights = lines.Select(JsonConvert.DeserializeObject<FlightNews>).ToArray();
            return flights.OrderBy(x => x.Offset).Where(x => x.Offset > offset).Take(count).ToArray();
        }

        public long? FindLatestOffset(string source)
        {
            var filename = GetFilename(source);
            if (!File.Exists(filename))
            {
                return null;
            }

            var lines = File.ReadAllLines(filename);
            var flights = lines.Select(JsonConvert.DeserializeObject<FlightNews>).ToArray();
            return flights.Max(x => x.Offset);
        }

        private static string GetFilename(string source)
        {
            return $"flights/{source}";
        }
    }
}