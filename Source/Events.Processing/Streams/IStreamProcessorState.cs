// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines the basis for the state of a <see cref="AbstractStreamProcessor" />.
    /// </summary>
    public interface IStreamProcessorState
    {
        /// <summary>
        /// Gets the <see cref="StreamPosition" />.
        /// </summary>
        StreamPosition Position { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="AbstractStreamProcessor" /> is partitioned or not.
        /// </summary>
        bool Partitioned { get; }
    }
}
