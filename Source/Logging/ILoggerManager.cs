// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Logging
{
    /// <summary>
    /// Defines a system that manages instances of <see cref="ILogger"/> and <see cref="ILogMessageWriterCreator"/>.
    /// </summary>
    public interface ILoggerManager
    {
        /// <summary>
        /// Adds creators that will be used to create log message writers for loggers.
        /// Also creates new writers for already created loggers.
        /// </summary>
        /// <remarks>
        /// This method requires locking new logger creation while updating already created loggers, and is so a fairly expensive operation.
        /// Therefore it is not meant to be used to change logging behaviour dynamically at runtime, but rather once during application startup.
        /// </remarks>
        /// <param name="creators">The instances of <see cref="ILogMessageWriterCreator"/> to add.</param>
        void AddLogMessageWriterCreators(params ILogMessageWriterCreator[] creators);

        /// <summary>
        /// Creates a logger for the provided type.
        /// </summary>
        /// <typeparam name="T">The type that the logger relates to.</typeparam>
        /// <returns>An <see cref="ILogMessageWriter"/>.</returns>
        ILogger<T> CreateLogger<T>();

        /// <summary>
        /// Creates a logger for the provided type.
        /// </summary>
        /// <param name="type">The type that the logger relates to.</param>
        /// <returns>An <see cref="ILogMessageWriter"/>.</returns>
        ILogger CreateLogger(Type type);
    }
}
