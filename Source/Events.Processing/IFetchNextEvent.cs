// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that can fetch the next <see cref="CommittedEventEnvelope">event</see>.
    /// </summary>
    public interface IFetchNextEvent
    {
        /// <summary>
        /// Get unprocessed stream.
        /// </summary>
        /// <param name="streamId"><see cref="StreamId">the stream in the event store</see>.</param>
        /// <param name="streamPosition"><see cref="StreamPosition">the position in the stream</see>.</param>
        /// <returns>The next event.</returns>
        Task<CommittedEventEnvelope> FetchNextEvent(StreamId streamId, StreamPosition streamPosition);
    }
}