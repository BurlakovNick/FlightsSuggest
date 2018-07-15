using FlightsSuggest.Core.Infrastructure.Vkontakte;

namespace FlightsSuggest.Core.Timelines
{
    public class FlightNewsFactory : IFlightNewsFactory
    {
        public FlightNews Create(VkWallPost vkWallPost, string source)
        {
            return new FlightNews
            {
                Id = vkWallPost.Id.ToString(),
                Date = vkWallPost.Date,
                Offset = vkWallPost.Date.Ticks,
                Source = source,
                Text = vkWallPost.Text,
                Url = vkWallPost.Url
            };
        }
    }
}