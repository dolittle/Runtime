// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;

namespace Dolittle.Runtime.Logging.Booting
{
    /// <summary>
    /// An implementation of <see cref="ILogMessageWriter"/> that writes log messages to a buffer to be written to an actual log after booting has completed.
    /// </summary>
    public class BootLogMessageWriter : ILogMessageWriter
    {
        readonly BootLogMessageWriterCreator _creator;
        readonly Type _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootLogMessageWriter"/> class.
        /// </summary>
        /// <param name="creator">The <see cref="BootLogMessageWriterCreator"/> to use for capturing the logs.</param>
        /// <param name="type">The type that the log messages relate to.</param>
        public BootLogMessageWriter(BootLogMessageWriterCreator creator, Type type)
        {
            _creator = creator;
            _type = type;
        }

        /// <inheritdoc/>
        public IDisposable BeginScope(string messageFormat, params object[] args)
            => Disposable.Empty;

        /// <inheritdoc/>
        public void Write(LogLevel logLevel, string message, params object[] args)
            => _creator.Capture(_type, logLevel, null, message, args);

        /// <inheritdoc/>
        public void Write(LogLevel logLevel, Exception exception, string message, params object[] args)
            => _creator.Capture(_type, logLevel, exception, message, args);
    }
}