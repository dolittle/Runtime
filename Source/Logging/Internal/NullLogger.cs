// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;

namespace Dolittle.Runtime.Logging.Internal
{
    /// <summary>
    /// Represents an implementation of <see cref="ILogger"/> that does nothing.
    /// </summary>
    public class NullLogger : ILogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullLogger"/> class.
        /// </summary>
        protected NullLogger()
        {
        }

        /// <summary>
        /// Gets the static singleton instance of <see cref="NullLogger"/>.
        /// </summary>
        public static ILogger Instance { get; } = new NullLogger();

        /// <inheritdoc/>
        public void Trace(string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Trace(Exception exception, string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Debug(string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Debug(Exception exception, string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Information(string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Information(Exception exception, string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Warning(string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Warning(Exception exception, string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Critical(string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Critical(Exception exception, string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Error(string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public void Error(Exception exception, string message, params object[] args)
        {
        }

        /// <inheritdoc/>
        public IDisposable BeginScope(string messageFormat, params object[] args) => Disposable.Empty;
    }
}
