// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Defines a special interface of <see cref="IFetchEventsFromStreams" /> that can fetch events only from well-known streams.
    /// </summary>
    public interface ICanFetchEventsFromWellKnownStreams : IFetchEventsFromStreams
    {
        /// <summary>
        /// Gets the well-known streams it can fetch Events from.
        /// </summary>
        IEnumerable<StreamId> WellKnownStreams { get; }

        /// <summary>
        /// Gets a value indicating whether this can fetch events from a given stream.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" /> to write to.</param>
        /// <returns>Whether it can fetch from specified stream.</returns>
        bool CanFetchFromStream(StreamId stream);
    }
}