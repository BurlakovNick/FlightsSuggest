using System;

namespace FlightsSuggest.ConsoleApp.Container
{
    public interface IContainerBuilder
    {
        IContainerBuilder RegisterModule(IContainerModule module);

        IServiceProvider Build();
    }
}