using FlightsSuggest.Core.Notifications;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateSetUpSearch(INotificationTrigger notificationTrigger);
        ICommand CreateSetUpSearchMenu();
        ICommand CreateLastNewsMenu();
        ICommand CreateLastNews(int lastNewsDays);
        ICommand CreateMenu();
    }
}