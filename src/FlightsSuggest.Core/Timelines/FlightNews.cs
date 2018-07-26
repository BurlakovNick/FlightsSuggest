using System;

namespace FlightsSuggest.Core.Timelines
{
    public class FlightNews
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public long Offset => Date.Ticks;
    }
}