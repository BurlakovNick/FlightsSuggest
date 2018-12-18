using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.ConsoleApp.Container
{
    public interface IContainerModule
    {
        void Load(IServiceCollection services);
    }
}