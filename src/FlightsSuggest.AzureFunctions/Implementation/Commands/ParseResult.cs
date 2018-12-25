namespace FlightsSuggest.AzureFunctions.Implementation.Commands
{
    public class ParseResult
    {
        private ParseResult(bool isSuccess, string errorMessage, ICommand command)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Command = command;
        }

        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public ICommand Command { get; }

        public static ParseResult Success(ICommand command)
        {
            return new ParseResult(true, string.Empty, command);
        }

        public static ParseResult Fail(string errorMessage)
        {
            return new ParseResult(false, errorMessage, null);
        }
    }
}