using FlightsSuggest.Core.Infrastructure.Vkontakte;

namespace FlightsSuggest.Core.Timelines
{
    public interface IFlightNewsFactory
    {
        FlightNews Create(VkWallPost vkWallPost, string source);
    }
}