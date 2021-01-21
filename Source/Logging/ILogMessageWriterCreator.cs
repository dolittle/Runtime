// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Logging
{
    /// <summary>
    /// Defines a system that can create instances of <see cref="ILogMessageWriter"/>.
    /// </summary>
    public interface ILogMessageWriterCreator
    {
        /// <summary>
        /// Creates a logger message writer for the provided type.
        /// </summary>
        /// <param name="type">The type that the logger relates to.</param>
        /// <returns>An <see cref="ILogMessageWriter"/>.</returns>
        ILogMessageWriter CreateFor(Type type);
    }
}
