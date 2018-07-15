namespace FlightsSuggest.Core.Configuration
{
    public interface IFlightsConfiguration
    {
        ulong VkApplicationId { get; }
        string VkAccessToken { get; }
    }
}