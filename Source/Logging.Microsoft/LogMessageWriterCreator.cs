// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Logging.Microsoft
{
    /// <summary>
    /// An implementation of <see cref="ILogMessageWriterCreator"/> that uses a <see cref="ILoggerFactory"/> to create instances of <see cref="ILogMessageWriter"/>.
    /// </summary>
    public class LogMessageWriterCreator : ILogMessageWriterCreator
    {
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageWriterCreator"/> class.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use to create log message writers.</param>
        public LogMessageWriterCreator(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc/>
        public ILogMessageWriter CreateFor(Type type)
            => new LogMessageWriter(_loggerFactory.CreateLogger(type));
    }
}
