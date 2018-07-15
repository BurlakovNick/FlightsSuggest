using System;

namespace FlightsSuggest.Core.Infrastructure.Vkontakte
{
    public class VkWallPost
    {
        private readonly long postId;
        private readonly long groupId;
        private readonly string groupName;

        public VkWallPost(long postId, long groupId, string text, string groupName, DateTime date)
        {
            this.postId = postId;
            this.groupId = groupId;
            this.groupName = groupName;
            this.Date = date;
            Text = text;
        }

        public string Text { get; }
        public DateTime Date { get; }
        public long Id => postId;

        public string Url => $"https://vk.com/{groupName}?w=wall{groupId}_{postId}";
    }
}