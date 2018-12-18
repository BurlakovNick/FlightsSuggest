using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.Testing.Container
{
    public interface IContainerModule
    {
        void Load(IServiceCollection services);
    }
}