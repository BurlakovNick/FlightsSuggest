using System;
using Microsoft.Extensions.Logging;

namespace FlightsSuggest.ConsoleApp.Implementation
{
    public class ConsoleLogger<T> : ILogger, ILogger<T>
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine($"{logLevel} {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new MyDisposable();
        }

        public class MyDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}