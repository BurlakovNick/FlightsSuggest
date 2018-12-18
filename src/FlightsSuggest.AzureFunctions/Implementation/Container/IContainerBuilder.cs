using System;

namespace FlightsSuggest.AzureFunctions.Implementation.Container
{
    public interface IContainerBuilder
    {
        IContainerBuilder RegisterModule(IContainerModule module);

        IServiceProvider Build();
    }
}