// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// Exception that gets thrown when a stream position is occupied while writing an event to a stream.
    /// </summary>
    public class StreamPositionOccupied : MongoException
    {
        const string TransientTransactionErrorLabel = "TransientTransactionError";

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamPositionOccupied"/> class.
        /// </summary>
        /// <param name="streamPosition">The stream position of the event that failed during write.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public StreamPositionOccupied(StreamPosition streamPosition, StreamId stream)
            : base($"Stream position '{streamPosition}' in stream '${stream}' is already occupied by another event.")
        {
            AddErrorLabel(TransientTransactionErrorLabel);
        }
    }
}