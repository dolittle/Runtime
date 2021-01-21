// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Logging.Booting
{
    /// <summary>
    /// Represents a buffered log message that was captured from a <see cref="ILogger"/> during booting.
    /// </summary>
    public class BootLogMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootLogMessage"/> class.
        /// </summary>
        /// <param name="type">The type that the log message relates to.</param>
        /// <param name="logLevel">The <see cref="LogLevel"/> of the message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">The format string of the log message in message template format.</param>
        /// <param name="arguments">The object array that contains zero or more objects to format.</param>
        public BootLogMessage(Type type, LogLevel logLevel, Exception exception, string message, object[] arguments)
        {
            Type = type;
            LogLevel = logLevel;
            Exception = exception;
            Message = message;
            Arguments = arguments;
        }

        /// <summary>
        /// Gets the type that the log message relates to.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the <see cref="LogLevel"/> of the message.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// Gets the exception to log.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the format string of the log message in message template format.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the object array that contains zero or more objects to format.
        /// </summary>
#pragma warning disable CA1819
        public object[] Arguments { get; }
#pragma warning restore CA1819
    }
}