using FlightsSuggest.Core.Configuration;

namespace FlightsSuggest.Testing
{
    public abstract class TestBase
    {
        protected static readonly IFlightsConfiguration Configuration = ConfigurationProvider.ProvideLocal();
    }
}