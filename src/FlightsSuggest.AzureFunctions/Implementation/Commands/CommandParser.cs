using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Notifications;
using Microsoft.Extensions.Logging;

namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public class CommandParser : ICommandParser
    {
        private readonly IFlightsConfiguration configuration;
        private readonly ILogger log;
        private readonly ICommandFactory commandFactory;

        public CommandParser(
            IFlightsConfiguration configuration,
            ILogger log,
            ICommandFactory commandFactory
            )
        {
            this.configuration = configuration;
            this.log = log;
            this.commandFactory = commandFactory;
        }

        public ParseResult Parse(string telegramMessage)
        {
            var message = telegramMessage?.ToLower() ?? string.Empty;

            if (message.StartsWith(configuration.TelegramSearchSettingWords))
            {
                log.LogInformation("Seen search setting words, lets set up search");

                var setting = message.Remove(0, configuration.TelegramSearchSettingWords.Length);
                var trigger = NotificationTriggers.BuildFromText(setting);

                if (!trigger.success)
                {
                    log.LogError($"Can't parse trigger expression. Error: {trigger.message}");
                    return ParseResult.Fail(trigger.message);
                }

                return ParseResult.Success(commandFactory.CreateSetUpSearch(trigger.result));
            }

            if (message == configuration.TelegramSearchSettingRequestWords)
            {
                return ParseResult.Success(commandFactory.CreateSetUpSearchMenu());
            }

            if (message == configuration.TelegramLastNewsRequestFormat)
            {
                return ParseResult.Success(commandFactory.CreateLastNewsMenu());
            }

            var lastNewsParseResult = PatternParser.ParseExpressionWithInt(configuration.TelegramLastNewsFormat, message);
            if (lastNewsParseResult.success)
            {
                return ParseResult.Success(commandFactory.CreateLastNews(lastNewsParseResult.result.Value));
            }

            return ParseResult.Success(commandFactory.CreateMenu());
        }
    }
}