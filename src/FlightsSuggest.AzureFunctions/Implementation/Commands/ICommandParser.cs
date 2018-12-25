namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public interface ICommandParser
    {
        ParseResult Parse(string telegramMessage);
    }
}