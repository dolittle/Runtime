// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Logging
{
    /// <summary>
    /// Defines an system that writes log messages to a log.
    /// </summary>
    public interface ILogMessageWriter
    {
        /// <summary>
        /// Formats and writes a log message.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/> of the message.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Write(LogLevel logLevel, string message, params object[] args);

        /// <summary>
        /// Formats and writes a log message.
        /// </summary>
        /// <param name="logLevel">The <see cref="LogLevel"/> of the message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Write(LogLevel logLevel, Exception exception, string message, params object[] args);

        /// <summary>
        /// Formats the message and creates a scope.
        /// </summary>
        /// <param name="messageFormat">The <see cref="string" >format string</see>of the log message in message template format.</param>
        /// <param name="args">The object array that contains zero or more objects to format.</param>
        /// <returns>A disposable scope object. Can be null.</returns>
        IDisposable BeginScope(string messageFormat, params object[] args);
    }
}
