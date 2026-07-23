#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TICO.GAUDI.Commons.Test
{
    internal sealed class RecordingLogger<T> : ILogger<T>
    {
        private sealed class EmptyScope : IDisposable
        {
            public static readonly EmptyScope Instance = new EmptyScope();

            public void Dispose()
            {
            }
        }

        public bool IsOutputEnabled { get; set; } = true;

        public List<RecordedLog> Entries { get; } = new List<RecordedLog>();

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
            => EmptyScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => IsOutputEnabled;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add(new RecordedLog(logLevel, formatter(state, exception), exception));
        }
    }

    internal sealed class RecordedLog
    {
        public RecordedLog(LogLevel level, string message, Exception? exception)
        {
            Level = level;
            Message = message;
            Exception = exception;
        }

        public LogLevel Level { get; }

        public string Message { get; }

        public Exception? Exception { get; }
    }
}
