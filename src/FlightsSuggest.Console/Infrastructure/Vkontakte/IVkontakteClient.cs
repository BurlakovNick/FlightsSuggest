namespace FlightsSuggest.ConsoleApp.Infrastructure.Vkontakte
{
    public interface IVkontakteClient
    {
        VkWallPost[] GetPosts(string groupName, ulong offset, ulong count);
    }
}