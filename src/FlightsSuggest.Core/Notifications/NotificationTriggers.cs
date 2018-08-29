using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace FlightsSuggest.Core.Notifications
{
    public static class NotificationTriggers
    {
        public static (INotificationTrigger result, bool success, string message) BuildFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return (new EmptyTrigger(), true, "empty");
            }

            var normalizedText = new string(text.Where(c => !char.IsWhiteSpace(c)).ToArray());

            var input = new Input(normalizedText);
            var parser = Expr;
            var parsed = parser(input);
            if (!parsed.WasSuccessful)
            {
                return (null, false, $"{parsed.Message}, Offset: {parsed.Remainder}");
            }
            return (parsed.Value, true, string.Empty);
        }

        private static Parser<INotificationTrigger> Word =>
            Parse
                .LetterOrDigit
                .AtLeastOnce()
                .Text()
                .Select(word => new TermNotificationTrigger(word));

        private static Parser<IEnumerable<INotificationTrigger>> List =>
            Parse.Ref(() => Expr)
                .DelimitedBy(Parse.Chars(',', ';'));

        private static Parser<INotificationTrigger> Should =>
            from left in Parse.Char('[')
            from expr in List.Optional()
            from right in Parse.Char(']')
            select new ShouldNotificationTrigger(expr.GetOrDefault()?.ToArray());

        private static Parser<INotificationTrigger> Must =>
            from left in Parse.Char('(')
            from expr in List.Optional()
            from right in Parse.Char(')')
            select new MustNotificationTrigger(expr.GetOrDefault()?.ToArray());

        private static Parser<INotificationTrigger> Expr => Should.XOr(Must).XOr(Word);
    }
}