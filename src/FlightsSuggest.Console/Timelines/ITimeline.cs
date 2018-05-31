using System.Collections.Generic;

namespace FlightsSuggest.ConsoleApp.Timelines
{
    public interface ITimeline
    {
        void Actualize();
        IEnumerable<FlightNews> ReadNews(long offset);
        long? LatestOffset { get; }
        string Name { get; }
    }
}