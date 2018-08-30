using Microsoft.Extensions.Configuration;

namespace FlightsSuggest.Core.Configuration
{
    public class FlightsConfiguration : IFlightsConfiguration
    {
        private readonly IConfiguration configuration;

        public FlightsConfiguration(
            IConfiguration configuration
        )
        {
            this.configuration = configuration;
        }

        public ulong VkApplicationId => ulong.Parse(configuration["VkApplicationId"]);
        public string VkAccessToken => configuration["VkAccessToken"];
        public string AzureTableConnectionString => configuration["AzureTableConnectionString"];
        public string TelegramBotToken => configuration["TelegramBotToken"];
        public string TelegramMagicWords => configuration["TelegramMagicWords"];
        public string TelegramSearchSettingRequestWords => configuration["TelegramSearchSettingRequestWords"];
        public string TelegramSearchSettingWords => configuration["TelegramSearchSettingWords"];
        public string TelegramLastNewsRequestFormat => configuration["TelegramLastNewsRequestFormat"];
        public string TelegramLastNewsFormat => configuration["TelegramLastNewsFormat"];
    }
}