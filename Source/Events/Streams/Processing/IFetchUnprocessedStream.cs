// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Streams.Processing
{
    /// <summary>
    /// Defines an interface for getting an unprocessed stream for an event processor.
    /// </summary>
    public interface IFetchUnprocessedStream
    {
        /// <summary>
        /// Get unprocessed stream.
        /// </summary>
        /// <param name="streamPosition"><see cref="StreamPosition">the position in the stream</see>.</param>
        /// <returns>An unprocessed stream.</returns>
        IObservable<CommittedEventEnvelope> GetUnprocessedStream(StreamPosition streamPosition);
    }
}