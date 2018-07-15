using FlightsSuggest.Core.Configuration;
using Microsoft.Azure.WebJobs;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public static class ConfigurationProvider
    {
        public static IFlightsConfiguration Provide(ExecutionContext context)
        {
            return Core.Configuration.ConfigurationProvider.Provide(context.FunctionAppDirectory);
        }
    }
}
