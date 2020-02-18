// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Defines a special interface of <see cref="IWriteEventsToStreams" /> that can write events only to well-known streams.
    /// </summary>
    public interface ICanWriteEventsToWellKnownStreams : IWriteEventsToStreams
    {
        /// <summary>
        /// Gets the well-known streams it can write Events to.
        /// </summary>
        IEnumerable<StreamId> WellKnownStreams { get; }

        /// <summary>
        /// Gets a value indicating whether this can write to a specific stream.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" /> to write to.</param>
        /// <returns>Whether it can write to specified stream.</returns>
        bool CanWriteToStream(StreamId stream);
    }
}