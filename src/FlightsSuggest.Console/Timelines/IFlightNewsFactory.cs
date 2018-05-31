using FlightsSuggest.ConsoleApp.Infrastructure.Vkontakte;

namespace FlightsSuggest.ConsoleApp.Timelines
{
    public interface IFlightNewsFactory
    {
        FlightNews Create(VkWallPost vkWallPost, string source);
    }
}