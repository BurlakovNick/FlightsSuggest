using System.Threading.Tasks;

namespace FlightsSuggest.ConsoleApp.Infrastructure.Vkontakte
{
    public interface IVkontakteClient
    {
        Task<VkWallPost[]> GetPostsAsync(string groupName, ulong offset, ulong count);
    }
}