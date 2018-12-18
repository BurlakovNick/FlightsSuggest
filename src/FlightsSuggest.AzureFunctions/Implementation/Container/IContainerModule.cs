using Microsoft.Extensions.DependencyInjection;

namespace FlightsSuggest.AzureFunctions.Implementation.Container
{
    public interface IContainerModule
    {
        void Load(IServiceCollection services);
    }
}