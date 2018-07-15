using System.Threading.Tasks;

namespace FlightsSuggest.Core.Infrastructure.Vkontakte
{
    public interface IVkontakteClient
    {
        Task<VkWallPost[]> GetPostsAsync(string groupName, ulong offset, ulong count);
    }
}