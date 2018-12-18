using System;

namespace FlightsSuggest.Testing.Container
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