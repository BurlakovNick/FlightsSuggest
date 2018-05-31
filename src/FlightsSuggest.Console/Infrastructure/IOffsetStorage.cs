namespace FlightsSuggest.ConsoleApp.Infrastructure
{
    public interface IOffsetStorage
    {
        void Write(string id, long offset);
        long? Find(string id);
    }
}