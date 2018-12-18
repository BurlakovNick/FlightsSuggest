using System;

namespace FlightsSuggest.ConsoleApp.Container
{
    public static class Container
    {
        public static IServiceProvider Build()
        {
            return new ContainerBuilder()
                .RegisterModule(new CoreAppModule())
                .Build();
        }
    }
}