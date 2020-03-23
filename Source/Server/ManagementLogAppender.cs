// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Logging.Json;
using Dolittle.Runtime.Logging.Management;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Execution.ExecutionContext;
using LogLevel = Dolittle.Logging.LogLevel;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents an implementation of <see cref="ILogAppender"/>.
    /// </summary>
    public class ManagementLogAppender : ILogAppender
    {
        internal static ILogManager LogManager;
        internal static IExecutionContextManager ExecutionContextManager;
        internal static ILoggerFactory LoggerFactory;
        readonly AsyncLocal<LoggingContext> _currentLoggingContext = new AsyncLocal<LoggingContext>();
        readonly LoggingContext _unknownLoggingContext = new LoggingContext();
        readonly Dictionary<string, Microsoft.Extensions.Logging.ILogger> _loggers = new Dictionary<string, Microsoft.Extensions.Logging.ILogger>();

        /// <inheritdoc/>
        public void Append(string filePath, int lineNumber, string member, LogLevel level, string message, Exception exception = null)
        {
            if (LoggerFactory == default || ExecutionContextManager == default || LogManager == default) return;

            Microsoft.Extensions.Logging.ILogger logger;

            var loggerKey = Path.GetFileNameWithoutExtension(filePath);
            if (!_loggers.ContainsKey(loggerKey))
            {
                logger = LoggerFactory.CreateLogger(loggerKey);
                _loggers[loggerKey] = logger;
            }
            else
            {
                logger = _loggers[loggerKey];
            }

            if (!logger.IsEnabled(Translate(level))) return;

            var logMessage = CreateLogMessage(filePath, lineNumber, member, message, LogLevelAsString(level), exception);
            LogManager?.Write(logMessage);
        }

        Microsoft.Extensions.Logging.LogLevel Translate(LogLevel level) => level switch
        {
            LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
            LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.Info => Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            LogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
            LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            _ => Microsoft.Extensions.Logging.LogLevel.None
        };

        string LogLevelAsString(LogLevel level)
        {
            return level switch
            {
                LogLevel.Critical => "fatal",
                LogLevel.Error => "error",
                LogLevel.Warning => "warn",
                LogLevel.Info => "info",
                LogLevel.Debug => "debug",
                LogLevel.Trace => "trace",
                _ => string.Empty,
            };
        }

        JsonLogMessage CreateLogMessage(string filePath, int lineNumber, string member, string message, string logLevel, Exception exception = null)
        {
            return new JsonLogMessage(
                logLevel,
                DateTimeOffset.Now,
                GetCurrentLoggingContext(),
                filePath,
                lineNumber,
                member,
                message,
                exception?.StackTrace ?? string.Empty);
        }

        LoggingContext GetCurrentLoggingContext()
        {
            if (LoggingContextIsSet())
            {
                SetLatestLoggingContext();
                return _currentLoggingContext.Value;
            }

            var executionContext = ExecutionContextManager?.Current;
            if (executionContext != default)
            {
                var loggingContext = CreateLoggingContextFrom(executionContext);
                _currentLoggingContext.Value = loggingContext;

                return loggingContext;
            }
            else
            {
                return _unknownLoggingContext;
            }
        }

        bool LoggingContextIsSet() => _currentLoggingContext?.Value != null;

        void SetLatestLoggingContext() => _currentLoggingContext.Value = CreateLoggingContextFrom(ExecutionContextManager?.Current);

        LoggingContext CreateLoggingContextFrom(ExecutionContext executionContext) =>
            new LoggingContext
            {
                Application = executionContext.Application,
                Microservice = executionContext.Microservice,
                CorrelationId = executionContext.CorrelationId,
                Environment = executionContext.Environment,
                TenantId = executionContext.Tenant
            };
    }
}