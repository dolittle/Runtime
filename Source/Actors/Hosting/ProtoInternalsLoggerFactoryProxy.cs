// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Actors.Hosting;

/// <summary>
/// Reduces noise from internal dependencies.
/// </summary>
class ProtoInternalsLoggerFactoryProxy : ILoggerFactory
{
    readonly ILoggerFactory _loggerFactory;

    public ProtoInternalsLoggerFactoryProxy(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public ILogger CreateLogger(string categoryName) => new LoggerProxy(_loggerFactory.CreateLogger(categoryName));

    public void AddProvider(ILoggerProvider provider)
    {
        _loggerFactory.AddProvider(provider);
    }

    public void Dispose()
    {
        _loggerFactory.Dispose();
    }

    class LoggerProxy : ILogger
    {
        readonly ILogger _logger;

        public LoggerProxy(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        static LogLevel GetLogLevel(LogLevel logLevel) =>
            logLevel switch
            {
                LogLevel.Debug => LogLevel.Trace,
                LogLevel.Information => LogLevel.Debug,
                _ => logLevel
            };

        public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(GetLogLevel(logLevel));

        public void Log(LogLevel logLevel, int eventId, object state,
            Exception exception, Func<object, Exception, string> formatter)
        {
            _logger.Log(GetLogLevel(logLevel), eventId, state, exception, formatter);
        }

        public IDisposable BeginScope<TState>(TState state) => _logger.BeginScope(state);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            _logger.Log(GetLogLevel(logLevel), eventId, state, exception, formatter);
    }
}
