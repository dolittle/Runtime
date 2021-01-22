// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Logging.Internal
{
    /// <summary>
    /// Represents an implementation of <see cref="ILogger"/> that does nothing.
    /// </summary>
    /// <typeparam name="T">The type that the log messages relate to.</typeparam>
    public class NullLogger<T> : NullLogger, ILogger<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullLogger{T}"/> class.
        /// </summary>
        protected NullLogger()
        {
        }

        /// <summary>
        /// Gets the static singleton instance of <see cref="NullLogger{T}"/>.
        /// </summary>
        public static ILogger<T> TypedInstance { get; } = new NullLogger<T>();
    }
}
