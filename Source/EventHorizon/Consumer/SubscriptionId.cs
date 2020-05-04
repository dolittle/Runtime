// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ApplicationModel;
using Dolittle.Concepts;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents an the unique identifier of an Event Horizon Subscription.
    /// </summary>
    public class SubscriptionId : Value<SubscriptionId>, IStreamProcessorId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionId"/> class.
        /// </summary>
        /// <param name="consumerTenant">The consumer <see cref="TenantId" />.</param>
        /// <param name="producerMicroservice">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        /// <param name="scope">The <see cref="Events.Store.ScopeId" />.</param>
        /// <param name="stream">The public <see cref="Events.Store.Streams.StreamId" /> to subscribe to.</param>
        /// <param name="partition">The <see cref="Events.Store.Streams.PartitionId" /> in the stream to subscribe to.</param>
        public SubscriptionId(TenantId consumerTenant, Microservice producerMicroservice, TenantId producerTenant, ScopeId scope, StreamId stream, PartitionId partition)
        {
            ConsumerTenantId = consumerTenant;
            ProducerMicroserviceId = producerMicroservice;
            ProducerTenantId = producerTenant;
            ScopeId = scope;
            StreamId = stream;
            PartitionId = partition;
        }

        /// <summary>
        /// Gets the consumer <see cref="TenantId" />.
        /// </summary>
        public TenantId ConsumerTenantId { get; }

        /// <summary>
        /// Gets the producer <see cref="Microservice" />.
        /// </summary>
        public Microservice ProducerMicroserviceId { get; }

        /// <summary>
        /// Gets the producer <see cref="TenantId" />.
        /// </summary>
        public TenantId ProducerTenantId { get; }

        /// <summary>
        /// Gets the <see cref="Events.Store.ScopeId" />.
        /// </summary>
        public ScopeId ScopeId { get; }

        /// <summary>
        /// Gets the public <see cref="Events.Store.Streams.StreamId" />.
        /// </summary>
        public StreamId StreamId { get; }

        /// <summary>
        /// Gets the <see cref="Events.Store.Streams.PartitionId" /> in the public stream.
        /// </summary>
        public PartitionId PartitionId { get; }

        /// <inheritdoc/>
        public override string ToString() => $"Consumer Tenant: '{ConsumerTenantId} Producer Microservice: '{ProducerMicroserviceId}' Producer Tenant: '{ProducerTenantId}' Scope: '{ScopeId}' Stream: '{StreamId}' Partition: '{PartitionId}''";
    }
}