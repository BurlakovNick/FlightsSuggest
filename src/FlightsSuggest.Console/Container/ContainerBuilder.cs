using System;
using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.ConsoleApp.Container
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection services;

        public ContainerBuilder()
        {
            services = new ServiceCollection();
        }

        public IContainerBuilder RegisterModule(IContainerModule module)
        {
            module.Load(services);
            return this;
        }

        public IServiceProvider Build()
        {
            return services.BuildServiceProvider();
        }
    }
}