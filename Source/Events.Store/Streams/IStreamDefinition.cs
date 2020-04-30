// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines the definition base definition of a Stream.
    /// </summary>
    public interface IStreamDefinition
    {
        /// <summary>
        /// Gets a value indicating whether the stream is partitioned.
        /// </summary>
        bool Partitioned { get; }

        /// <summary>
        /// Gets a value indicating whether this is a public stream.
        /// </summary>
        bool Public { get; }
    }
}
