// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Defines a special interface of <see cref="IFetchEventsFromStreams" /> that can fetch events only from well-known streams.
    /// </summary>
    public interface ICanGetMetadataFromWellKnownStreams : IStreamsMetadata
    {
        /// <summary>
        /// Gets the well-known streams it can fetch Events from.
        /// </summary>
        IEnumerable<StreamId> WellKnownStreams { get; }

        /// <summary>
        /// Gets a value indicating whether this can get metadata from a given stream.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" /> to get metadata from.</param>
        /// <returns>Whether it can get metadata from specified stream.</returns>
        bool CanGetMetadataFromStream(StreamId stream);
    }
}