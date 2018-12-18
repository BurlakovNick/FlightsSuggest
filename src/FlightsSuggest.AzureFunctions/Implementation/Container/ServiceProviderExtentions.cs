using System;
using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.AzureFunctions.Implementation.Container
{
    public static class ServiceProviderExtentions
    {
        public static FlightNotifier GetFlightNotifier(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<FlightNotifier>();
        }
    }
}