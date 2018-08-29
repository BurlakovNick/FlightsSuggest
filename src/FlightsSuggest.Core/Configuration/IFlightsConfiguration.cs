namespace FlightsSuggest.Core.Configuration
{
    public interface IFlightsConfiguration
    {
        ulong VkApplicationId { get; }
        string VkAccessToken { get; }
        string AzureTableConnectionString { get; }
        string TelegramBotToken { get; }
        string TelegramMagicWords { get; }
        string TelegramSearchSettingWords { get; }
        string TelegramLastNewsFormat { get; }
    }
}