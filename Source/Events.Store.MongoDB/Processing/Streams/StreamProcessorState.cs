// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson.Serialization.Attributes;
using runtime = Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents the state of an <see cref="runtime.AbstractScopedStreamProcessor" />.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class StreamProcessorState : AbstractStreamProcessorState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorState"/> class.
        /// </summary>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        /// <param name="position">The position.</param>
        /// <param name="retryTime">The time to retry processing.</param>
        /// <param name="failureReason">The reason for failing.</param>
        /// <param name="processingAttempts">The number of times the event at <see cref="AbstractStreamProcessorState.Position" /> has been processed.</param>
        /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
        /// <param name="isFailing">Whether the stream processor is failing.</param>
        public StreamProcessorState(Guid eventProcessorId, Guid sourceStreamId, ulong position, DateTime retryTime, string failureReason, uint processingAttempts, DateTime lastSuccessfullyProcessed, bool isFailing)
            : base(eventProcessorId, sourceStreamId, position, lastSuccessfullyProcessed)
        {
            RetryTime = retryTime;
            FailureReason = failureReason;
            ProcessingAttempts = processingAttempts;
            IsFailing = isFailing;
        }

        /// <summary>
        /// Gets or sets the retry time with Kind of UTC.
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime RetryTime { get; set; }

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

        /// <inheritdoc/>
        public override IStreamProcessorState ToRuntimeRepresentation() =>
            new runtime.StreamProcessorState(
                Position,
                FailureReason,
                RetryTime,
                ProcessingAttempts,
                LastSuccessfullyProcessed,
                IsFailing);
    }
}
