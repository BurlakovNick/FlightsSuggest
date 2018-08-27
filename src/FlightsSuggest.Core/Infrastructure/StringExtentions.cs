using System.Linq;

namespace FlightsSuggest.Core.Infrastructure
{
    public static class StringExtentions
    {
        public static string WithoutWhitespaces(this string str) => new string(str.Where(x => !char.IsWhiteSpace(x)).ToArray());
    }
}