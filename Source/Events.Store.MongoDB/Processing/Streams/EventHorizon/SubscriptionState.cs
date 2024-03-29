// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams.EventHorizon;

/// <summary>
/// Represents the state of an <see cref="SubscriptionState" />.
/// </summary>
[BsonIgnoreExtraElements]
public class SubscriptionState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionState"/> class.
    /// </summary>
    /// <param name="producerMicroserviceId">The producer <see cref="Microservice" />.</param>
    /// <param name="producerTenantId">The producer <see cref="TenantId" />.</param>
    /// <param name="streamId">The public <see cref="Store.Streams.StreamId" /> to subscribe to.</param>
    /// <param name="partitionId">The <see cref="Store.Streams.PartitionId" /> in the stream to subscribe to.</param>
    /// <param name="position">The public stream position.</param>
    /// <param name="eventLogSequence">The event log position</param>
    /// <param name="retryTime">The time to retry processing.</param>
    /// <param name="failureReason">The reason for failing.</param>
    /// <param name="processingAttempts">The number of times the event at <see cref="Position" /> has been processed.</param>
    /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
    /// <param name="isFailing">Whether the Stream Processor is failing.</param>
    public SubscriptionState(
        Guid producerMicroserviceId,
        Guid producerTenantId,
        Guid streamId,
        string partitionId,
        ulong position,
        ulong eventLogSequence,
        DateTime retryTime,
        string failureReason,
        uint processingAttempts,
        DateTime lastSuccessfullyProcessed,
        bool isFailing)
    {
        Microservice = producerMicroserviceId;
        Tenant = producerTenantId;
        Stream = streamId;
        Partition = partitionId;
        Position = position;
        EventLogSequence = eventLogSequence;
        LastSuccessfullyProcessed = lastSuccessfullyProcessed;
        RetryTime = retryTime;
        FailureReason = failureReason;
        ProcessingAttempts = processingAttempts;
        IsFailing = isFailing;
    }

    /// <summary>
    /// Gets or sets the producer <see cref="Microservice" />.
    /// </summary>
    public Guid Microservice { get; set; }

    /// <summary>
    /// Gets or sets the producer <see cref="TenantId" />.
    /// </summary>
    public Guid Tenant { get; set; }

    /// <summary>
    /// Gets or sets the public <see cref="Store.Streams.StreamId" />.
    /// </summary>
    public Guid Stream { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Store.Streams.PartitionId" /> in the public stream.
    /// </summary>
    public string Partition { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong Position { get; set; }
    
    /// <summary>
    /// Gets or sets the EventLogSequence.
    /// </summary>
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong EventLogSequence { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the StreamProcessor has processed the stream with Kind of UTC.
    /// </summary>
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime LastSuccessfullyProcessed { get; set; }

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
}
