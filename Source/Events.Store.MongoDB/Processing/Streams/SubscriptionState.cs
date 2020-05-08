// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.ApplicationModel;
using Dolittle.Tenancy;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents the state of an <see cref="SubscriptionState" />.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class SubscriptionState : AbstractSubscriptionState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionState"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId" />.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId" />.</param>
        /// <param name="scope">The <see cref="Store.ScopeId" />.</param>
        /// <param name="streamId">The public <see cref="Store.Streams.StreamId" /> to subscribe to.</param>
        /// <param name="partitionId">The <see cref="Store.Streams.PartitionId" /> in the stream to subscribe to.</param>
        /// <param name="position">The position.</param>
        /// <param name="retryTime">The time to retry processing.</param>
        /// <param name="failureReason">The reason for failing.</param>
        /// <param name="processingAttempts">The number of times the event at <see cref="AbstractSubscriptionState.Position" /> has been processed.</param>
        /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
        public SubscriptionState(
            Guid consumerTenantId,
            Guid producerMicroserviceId,
            Guid producerTenantId,
            Guid scope,
            Guid streamId,
            Guid partitionId,
            ulong position,
            DateTimeOffset retryTime,
            string failureReason,
            uint processingAttempts,
            DateTimeOffset lastSuccessfullyProcessed)
            : base(consumerTenantId, producerMicroserviceId, producerTenantId, scope, streamId, partitionId, position, lastSuccessfullyProcessed)
        {
            RetryTime = retryTime;
            FailureReason = failureReason;
            ProcessingAttempts = processingAttempts;
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
    }
}
