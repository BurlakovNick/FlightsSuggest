namespace FlightsSuggest.Core.Configuration
{
    public interface IFlightsConfiguration
    {
        ulong VkApplicationId { get; }
        string VkAccessToken { get; }
        string AzureTableConnectionString { get; }
        string TelegramBotToken { get; }
        string TelegramMagicWords { get; }
        string TelegramSearchSettingRequestWords { get; }
        string TelegramSearchSettingWords { get; }
        string TelegramLastNewsRequestFormat { get; }
        string TelegramLastNewsFormat { get; }

        string AzureFunctionsUrl { get; }
        string ShowSubscribersUrl { get; }
    }
}