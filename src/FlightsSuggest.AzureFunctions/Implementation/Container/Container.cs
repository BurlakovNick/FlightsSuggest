using System;
using Microsoft.Azure.WebJobs;

namespace FlightsSuggest.AzureFunctions.Implementation.Container
{
    public static class Container
    {
        public static IServiceProvider Build(ExecutionContext executionContext)
        {
            return new ContainerBuilder()
                .RegisterModule(new CoreAppModule(executionContext))
                .Build();
        }
    }
}