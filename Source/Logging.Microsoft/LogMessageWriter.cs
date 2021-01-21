// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using MicrosoftLogger = Microsoft.Extensions.Logging.ILogger;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Dolittle.Runtime.Logging.Microsoft
{
    /// <summary>
    /// An implementation of <see cref="ILogMessageWriter"/> that uses a <see cref="MicrosoftLogger"/> to write log messages.
    /// </summary>
    internal class LogMessageWriter : ILogMessageWriter
    {
        readonly MicrosoftLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageWriter"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="MicrosoftLogger"/> to use to write log messages.</param>
        public LogMessageWriter(MicrosoftLogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public IDisposable BeginScope(string messageFormat, params object[] args)
            => _logger.BeginScope(messageFormat, args);

        /// <inheritdoc/>
        public void Write(LogLevel logLevel, string message, params object[] args)
            => _logger.Log(TranslateLogLevel(logLevel), message, args);

        /// <inheritdoc/>
        public void Write(LogLevel logLevel, Exception exception, string message, params object[] args)
            => _logger.Log(TranslateLogLevel(logLevel), exception, message, args);

        MicrosoftLogLevel TranslateLogLevel(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Error => MicrosoftLogLevel.Error,
            LogLevel.Critical => MicrosoftLogLevel.Critical,
            LogLevel.Warning => MicrosoftLogLevel.Warning,
            LogLevel.Info => MicrosoftLogLevel.Information,
            LogLevel.Debug => MicrosoftLogLevel.Debug,
            LogLevel.Trace => MicrosoftLogLevel.Trace,
            _ => MicrosoftLogLevel.None,
        };
    }
}
