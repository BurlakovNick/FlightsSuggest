using System;
using System.Linq;
using VkNet;
using VkNet.Model.RequestParams;

namespace FlightsSuggest.ConsoleApp.Infrastructure.Vkontakte
{
    public class VkontakteClient : IVkontakteClient
    {
        private readonly Lazy<VkApi> client;

        public VkontakteClient(
            ulong applicationId,
            string accessToken
        )
        {
            client = new Lazy<VkApi>(() => CreateClient(applicationId, accessToken));
        }

        public VkWallPost[] GetPosts(string groupName, ulong offset, ulong count)
        {
            var wall = client.Value.Wall.Get(new WallGetParams
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

        private static VkApi CreateClient(ulong applicationId, string accessToken)
        {
            var vkApi = new VkApi();

            vkApi.Authorize(new ApiAuthParams
            {
                ApplicationId = applicationId,
                AccessToken = accessToken
            });

            return vkApi;
        }
    }
}