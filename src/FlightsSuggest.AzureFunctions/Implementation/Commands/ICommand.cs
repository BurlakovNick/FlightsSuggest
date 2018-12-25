using System.Threading.Tasks;
using FlightsSuggest.Core.Notifications;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public interface ICommand
    {
        CommandType Type { get; }
        Task ExecuteAsync(Subscriber subscriber, long chatId);
    }
}