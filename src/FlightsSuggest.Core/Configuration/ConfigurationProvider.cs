using System.IO;
using Microsoft.Extensions.Configuration;

namespace FlightsSuggest.Core.Configuration
{
    public static class ConfigurationProvider
    {
        public static IFlightsConfiguration ProvideLocal()
        {
            return Provide(Directory.GetCurrentDirectory());
        }

        public static IFlightsConfiguration Provide(string directory, string settingsFilename = "settings.json")
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile(settingsFilename, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            return new FlightsConfiguration(configuration);
        }
    }
}