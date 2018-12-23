using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FlightsSuggest.AzureFunctions.Implementation.Container
{
    public static class Container
    {
        public static IServiceProvider Build(ExecutionContext executionContext, ILogger logger)
        {
            return new ContainerBuilder()
                .RegisterModule(new CoreAppModule(executionContext, logger))
                .Build();
        }
    }
}