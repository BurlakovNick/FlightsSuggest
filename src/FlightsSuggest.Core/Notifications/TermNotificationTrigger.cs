using FlightsSuggest.Core.Timelines;

namespace FlightsSuggest.Core.Notifications
{
    public class TermNotificationTrigger : INotificationTrigger
    {
        private readonly string term;

        public TermNotificationTrigger(string term)
        {
            this.term = term;
        }

        public bool ShouldNotify(FlightNews flightNews)
        {
            return flightNews.Text.Contains(term);
        }
    }
}