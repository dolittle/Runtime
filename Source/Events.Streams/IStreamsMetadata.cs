// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Defines a system that knows how to derive the metadata for streams.
    /// </summary>
    public interface IStreamsMetadata
    {
        /// <summary>
        /// Gets the last processed <see cref="EventLogSequenceNumber" /> in a stream.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="EventLogSequenceNumber" /> or null if there are no events in the stream..</returns>
        Task<EventLogSequenceNumber> GetLastProcessedEventLogSequenceNumber(StreamId stream, CancellationToken cancellationToken);
    }
}