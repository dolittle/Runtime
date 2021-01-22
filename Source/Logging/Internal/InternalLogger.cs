// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Disposables;

namespace Dolittle.Runtime.Logging.Internal
{
    /// <summary>
    /// Represents an implementation of <see cref="ILogger"/>.
    /// </summary>
    public class InternalLogger : ILogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalLogger"/> class.
        /// </summary>
        protected InternalLogger()
        {
        }

        /// <summary>
        /// Gets or sets the instances of <see cref="ILogMessageWriter"/> to use for writing log messages.
        /// </summary>
#pragma warning disable CA1819
        public ILogMessageWriter[] LogMessageWriters { get; set; }
#pragma warning restore CA1819

        /// <inheritdoc/>
        public void Trace(string message, params object[] args) => Write(LogLevel.Trace, message, args);

        /// <inheritdoc/>
        public void Trace(Exception exception, string message, params object[] args) => Write(LogLevel.Trace, exception, message, args);

        /// <inheritdoc/>
        public void Debug(string message, params object[] args) => Write(LogLevel.Debug, message, args);

        /// <inheritdoc/>
        public void Debug(Exception exception, string message, params object[] args) => Write(LogLevel.Debug, exception, message, args);

        /// <inheritdoc/>
        public void Information(string message, params object[] args) => Write(LogLevel.Info, message, args);

        /// <inheritdoc/>
        public void Information(Exception exception, string message, params object[] args) => Write(LogLevel.Info, exception, message, args);

        /// <inheritdoc/>
        public void Warning(string message, params object[] args) => Write(LogLevel.Warning, message, args);

        /// <inheritdoc/>
        public void Warning(Exception exception, string message, params object[] args) => Write(LogLevel.Warning, exception, message, args);

        /// <inheritdoc/>
        public void Critical(string message, params object[] args) => Write(LogLevel.Critical, message, args);

        /// <inheritdoc/>
        public void Critical(Exception exception, string message, params object[] args) => Write(LogLevel.Critical, exception, message, args);

        /// <inheritdoc/>
        public void Error(string message, params object[] args) => Write(LogLevel.Error, message, args);

        /// <inheritdoc/>
        public void Error(Exception exception, string message, params object[] args) => Write(LogLevel.Error, exception, message, args);

        /// <inheritdoc/>
        public IDisposable BeginScope(string messageFormat, params object[] args)
            => new CompositeDisposable(LogMessageWriters.Select(_ => _.BeginScope(messageFormat, args)).Where(_ => _ != default).ToArray());

        /// <summary>
        /// Writes the log message to the log message writers in <see cref="LogMessageWriters"/>.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/> of the message.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        protected virtual void Write(LogLevel logLevel, string message, params object[] args)
        {
            var writers = LogMessageWriters;
            if (writers == null) return;

            for (var i = 0; i < writers.Length; ++i)
            {
                try
                {
                    writers[i].Write(logLevel, message, args);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Writes the log message with an exception to the log message writers in <see cref="LogMessageWriters"/>.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/> of the message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        protected virtual void Write(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            var writers = LogMessageWriters;
            if (writers == null) return;

            for (var i = 0; i < writers.Length; ++i)
            {
                try
                {
                    writers[i].Write(logLevel, exception, message, args);
                }
                catch
                {
                }
            }
        }
    }
}
