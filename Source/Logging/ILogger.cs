// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Logging
{
    /// <summary>
    /// Defines a system for writing log messages.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Trace(string message, params object[] args);

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Trace(Exception exception, string message, params object[] args);

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Debug(Exception exception, string message, params object[] args);

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Information(string message, params object[] args);

        /// <summary>
        /// Formats and writes a information log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Information(Exception exception, string message, params object[] args);

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Warning(string message, params object[] args);

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Warning(Exception exception, string message, params object[] args);

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Critical(string message, params object[] args);

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Critical(Exception exception, string message, params object[] args);

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Formats and writes a error log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        void Error(Exception exception, string message, params object[] args);

        /// <summary>
        /// Formats the message and creates a scope.
        /// </summary>
        /// <param name="messageFormat">The <see cref="string" >format string</see>of the log message in message template format.</param>
        /// <param name="args">The object array that contains zero or more objects to format.</param>
        /// <returns>A disposable scope object. Can be null.</returns>
        IDisposable BeginScope(string messageFormat, params object[] args);
    }
}
