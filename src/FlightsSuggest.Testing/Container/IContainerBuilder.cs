using System;

namespace FlightsSuggest.Testing.Container
{
    public interface IContainerBuilder
    {
        IContainerBuilder RegisterModule(IContainerModule module);

        IServiceProvider Build();
    }
}