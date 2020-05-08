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
    /// Represents the base state of an <see cref="SubscriptionState" />.
    /// </summary>
    [BsonDiscriminator(RootClass = true, Required = true)]
    [BsonKnownTypes(typeof(SubscriptionState))]
    [BsonIgnoreExtraElements]
    public abstract class AbstractSubscriptionState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSubscriptionState"/> class.
        /// </summary>
        /// <param name="consumerTenantId">The consumer <see cref="TenantId" />.</param>
        /// <param name="producerMicroserviceId">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenantId">The producer <see cref="TenantId" />.</param>
        /// <param name="scope">The <see cref="Store.ScopeId" />.</param>
        /// <param name="streamId">The public <see cref="Store.Streams.StreamId" /> to subscribe to.</param>
        /// <param name="partitionId">The <see cref="Store.Streams.PartitionId" /> in the stream to subscribe to.</param>
        /// <param name="position">The position.</param>
        /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
        protected AbstractSubscriptionState(
            Guid consumerTenantId,
            Guid producerMicroserviceId,
            Guid producerTenantId,
            Guid scope,
            Guid streamId,
            Guid partitionId,
            ulong position,
            DateTimeOffset lastSuccessfullyProcessed)
        {
            ConsumerTenantId = consumerTenantId;
            ProducerMicroserviceId = producerMicroserviceId;
            ProducerTenantId = producerTenantId;
            ScopeId = scope;
            StreamId = streamId;
            PartitionId = partitionId;
            Position = position;
            LastSuccessfullyProcessed = lastSuccessfullyProcessed;
        }

        /// <summary>
        /// Gets or sets the consumer <see cref="TenantId" />.
        /// </summary>
        public Guid ConsumerTenantId { get; set; }

        /// <summary>
        /// Gets or sets the producer <see cref="Microservice" />.
        /// </summary>
        public Guid ProducerMicroserviceId { get; set; }

        /// <summary>
        /// Gets or sets the producer <see cref="TenantId" />.
        /// </summary>
        public Guid ProducerTenantId { get; set; }

        /// <summary>
        /// Gets or sets the scope id.
        /// </summary>
        public Guid ScopeId { get; set; }

        /// <summary>
        /// Gets or sets the public <see cref="Store.Streams.StreamId" />.
        /// </summary>
        public Guid StreamId { get; set; }

        /// <summary>
        /// Gets or setsthe <see cref="Store.Streams.PartitionId" /> in the public stream.
        /// </summary>
        public Guid PartitionId { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        [BsonRepresentation(BsonType.Decimal128)]
        public ulong Position { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the StreamProcessor has processed the stream.
        /// </summary>
        [BsonRepresentation(BsonType.Document)]
        public DateTimeOffset LastSuccessfullyProcessed { get; set; }
    }
}
