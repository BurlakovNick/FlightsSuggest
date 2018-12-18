using System;
using System.Threading.Tasks;

namespace FlightsSuggest.Core.Notifications
{
    public interface INotifier
    {
        Task NotifyAsync(Subscriber[] subscribers);
        Task NotifyAsync(Subscriber subscriber);
        Task RewindOffsetAsync(string subscriberId, string timelineName, long offset);
        Task<(string timelineName, DateTime? offset)[]> SelectOffsetsAsync(string subscriberId);
    }
}