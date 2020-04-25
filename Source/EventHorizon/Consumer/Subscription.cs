// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ApplicationModel;
using Dolittle.Concepts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents an event horizon.
    /// </summary>
    public class Subscription : Value<Subscription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="consumerTenant">The consumer <see cref="TenantId" />.</param>
        /// <param name="producerMicroservice">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The public <see cref="StreamId" /> to subscribe to.</param>
        /// <param name="partition">The <see cref="PartitionId" /> in the stream to subscribe to.</param>
        public Subscription(TenantId consumerTenant, Microservice producerMicroservice, TenantId producerTenant, ScopeId scope, StreamId stream, PartitionId partition)
        {
            ConsumerTenant = consumerTenant;
            ProducerMicroservice = producerMicroservice;
            ProducerTenant = producerTenant;
            Scope = scope;
            Stream = stream;
            Partition = partition;
        }

        /// <summary>
        /// Gets the consumer <see cref="TenantId" />.
        /// </summary>
        public TenantId ConsumerTenant { get; }

        /// <summary>
        /// Gets the producer <see cref="Microservice" />.
        /// </summary>
        public Microservice ProducerMicroservice { get; }

        /// <summary>
        /// Gets the producer <see cref="TenantId" />.
        /// </summary>
        public TenantId ProducerTenant { get; }

        /// <summary>
        /// Gets the <see cref="ScopeId" />.
        /// </summary>
        public ScopeId Scope { get; }

        /// <summary>
        /// Gets the public <see cref="StreamId" />.
        /// </summary>
        public StreamId Stream { get; }

        /// <summary>
        /// Gets the <see cref="PartitionId" /> in the public stream.
        /// </summary>
        public PartitionId Partition { get; }

        /// <inheritdoc/>
        public override string ToString() => $"Consumer Tenant: '{ConsumerTenant} Producer Microservice: '{ProducerMicroservice}' Producer Tenant: '{ProducerTenant}' Scope: '{Scope}' Stream: '{Stream}' Partition: '{Partition}''";
    }
}