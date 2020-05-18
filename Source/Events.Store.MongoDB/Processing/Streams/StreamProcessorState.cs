// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents the state of an <see cref="Runtime.Events.Processing.Streams.AbstractScopedStreamProcessor" />.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class StreamProcessorState : AbstractStreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
        /// </summary>
        /// <param name="scopeId">The <see cref="ScopeId" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        /// <param name="position">The position.</param>
        /// <param name="retryTime">The time to retry processing.</param>
        /// <param name="failureReason">The reason for failing.</param>
        /// <param name="processingAttempts">The number of times the event at <see cref="AbstractStreamProcessorState.Position" /> has been processed.</param>
        /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
        /// <param name="isFailing">Whether the stream processor is failing.</param>
        public StreamProcessorState(Guid scopeId, Guid eventProcessorId, Guid sourceStreamId, ulong position, DateTimeOffset retryTime, string failureReason, uint processingAttempts, DateTimeOffset lastSuccessfullyProcessed, bool isFailing)
            : base(scopeId, eventProcessorId, sourceStreamId, position, lastSuccessfullyProcessed)
        {
            RetryTime = retryTime;
            FailureReason = failureReason;
            ProcessingAttempts = processingAttempts;
            IsFailing = isFailing;
        }

        /// <summary>
        /// Gets or sets the retry time.
        /// </summary>
        [BsonRepresentation(BsonType.Document)]
        public DateTimeOffset RetryTime { get; set; }

        /// <summary>
        /// Gets or sets the reason for failure.
        /// </summary>
        public string FailureReason { get; set; }

        /// <summary>
        /// Gets or sets the number of times that the event at position has been attempted processed.
        /// </summary>
        public uint ProcessingAttempts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Stream Processor is failing or not.
        /// </summary>
        public bool IsFailing { get; set; }

        /// <summary>
        /// Converts the <see cref="StreamProcessorState" /> to the runtime representation of <see cref="Runtime.Events.Processing.Streams.StreamProcessorState"/>.
        /// </summary>
        /// <returns>The converted <see cref="Runtime.Events.Processing.Streams.Partitioned.StreamProcessorState" />.</returns>
        public override IStreamProcessorState ToRuntimeRepresentation() =>
            new Runtime.Events.Processing.Streams.StreamProcessorState(
                Position,
                FailureReason,
                RetryTime,
                ProcessingAttempts,
                LastSuccessfullyProcessed,
                IsFailing);
    }
}
