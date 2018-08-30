using System;
using Sprache;

namespace FlightsSuggest.Core.Infrastructure
{
    public static class PatternParser
    {
        public static (int? result, bool success, string message) ParseExpressionWithInt(string format, string text)
        {
            var tokens = format.Split(new [] {"{int}"}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2)
            {
                throw new InvalidOperationException($"Can't parse format string {format}. Expected only one {{int}}");
            }

            var parser = from left in Parse.String(tokens[0])
                from value in Parse.Number
                from right in Parse.String(tokens[1])
                select value;

            var parseResult = parser(new Input(text));
            if (!parseResult.WasSuccessful || string.IsNullOrEmpty(parseResult.Value))
            {
                return (null, false, parseResult.Message);
            }

            return (int.Parse(parseResult.Value), true, string.Empty);
        }

        public static string ReplacePatternWithInt(string format, int value) => format.Replace("{int}", value.ToString());
    }
}