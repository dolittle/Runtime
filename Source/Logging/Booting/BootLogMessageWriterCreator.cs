// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;

namespace Dolittle.Runtime.Logging.Booting
{
    /// <summary>
    /// An implementation of <see cref="ILogMessageWriterCreator"/> that creates instances of <see cref="BootLogMessageWriter"/>.
    /// </summary>
    public class BootLogMessageWriterCreator : ILogMessageWriterCreator
    {
        readonly ConcurrentQueue<BootLogMessage> _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootLogMessageWriterCreator"/> class.
        /// </summary>
        public BootLogMessageWriterCreator()
        {
            _messages = new ConcurrentQueue<BootLogMessage>();
        }

        /// <inheritdoc/>
        public ILogMessageWriter CreateFor(Type type) => new BootLogMessageWriter(this, type);

        /// <summary>
        /// Captures a log message.
        /// </summary>
        /// <param name="type">The type that the log message relates to.</param>
        /// <param name="logLevel">The <see cref="LogLevel"/> of the message.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void Capture(Type type, LogLevel logLevel, Exception exception, string message, params object[] args)
            => _messages.Enqueue(new BootLogMessage(type, logLevel, exception, message, args));

        /// <summary>
        /// Flushes captured log messages to the provided log message writers.
        /// </summary>
        /// <param name="writersForType">The delegate that is used to provide instances of <see cref="ILogMessageWriter"/> for the captured log messages.</param>
        public void FlushTo(Func<Type, ILogMessageWriter[]> writersForType)
        {
            while (_messages.TryDequeue(out var message))
            {
                var writers = writersForType(message.Type);

                for (var i = 0; i < writers.Length; ++i)
                {
                    try
                    {
                        if (message.Exception == null) writers[i].Write(message.LogLevel, message.Message, message.Arguments);
                        else writers[i].Write(message.LogLevel, message.Exception, message.Message, message.Arguments);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}