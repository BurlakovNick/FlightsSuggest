using System;
using System.Linq;
using Newtonsoft.Json;
using VkNet;
using VkNet.Model.RequestParams;

namespace FlightsSuggest.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var vkApi = new VkApi();

            vkApi.Authorize(new ApiAuthParams
            {
                ApplicationId = (ulong)int.Parse(args[0]),
                AccessToken = args[1]
            });
            Console.WriteLine(vkApi.Token);

            var wall = vkApi.Wall.Get(new WallGetParams
            {
                Count = 5,
                Offset = 0,
                Domain = "vandroukiru",
            });

            var posts = wall
                .WallPosts
                .Select(x => new VkWallPost
                {
                    Id = x.Id,
                    GroupId = x.OwnerId,
                    Text = x.Text
                })
                .ToArray();

            Console.WriteLine(JsonConvert.SerializeObject(posts));
        }
    }

    public class VkWallPost
    {
        public long? Id { get; set; }
        public long? GroupId { get; set; }
        public string Text { get; set; }
    }
}