namespace FlightsSuggest.ConsoleApp.Timelines
{
    public interface IFlightNewsStorage
    {
        void Write(FlightNews flight);
        FlightNews[] Select(long offset, int count, string source);
        long? FindLatestOffset(string source);
    }
}