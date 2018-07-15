using System;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model.RequestParams;

namespace FlightsSuggest.ConsoleApp.Infrastructure.Vkontakte
{
    public class VkontakteClient : IVkontakteClient
    {
        private readonly Lazy<Task<VkApi>> client;

        public VkontakteClient(
            ulong applicationId,
            string accessToken
        )
        {
            client = new Lazy<Task<VkApi>>(() => CreateClientAsync(applicationId, accessToken));
        }

        public async Task<VkWallPost[]> GetPostsAsync(string groupName, ulong offset, ulong count)
        {
            var vkClient = await client.Value;
            var wall = await vkClient.Wall.GetAsync(new WallGetParams
            {
                //note: 1 for pinned post
                Count = count + 1,
                Offset = offset,
                Domain = groupName,
            });

            return wall
                .WallPosts
                .Where(x => !x.IsPinned.HasValue || !x.IsPinned.Value)
                .Select(x => new VkWallPost(x.Id.Value, x.OwnerId.Value, x.Text, groupName, x.Date.Value))
                .Take((int)count)
                .ToArray();
        }

        private static async Task<VkApi> CreateClientAsync(ulong applicationId, string accessToken)
        {
            var vkApi = new VkApi();

            await vkApi.AuthorizeAsync(new ApiAuthParams
            {
                ApplicationId = applicationId,
                AccessToken = accessToken
            });

            return vkApi;
        }
    }
}