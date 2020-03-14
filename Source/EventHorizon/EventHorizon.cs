// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Concepts;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents an event horizon.
    /// </summary>
    public class EventHorizon : Value<EventHorizon>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizon"/> class.
        /// </summary>
        /// <param name="consumerMicroservice">The consumer <see cref="Microservice" />.</param>
        /// <param name="consumerTenant">The consumer <see cref="TenantId" />.</param>
        /// <param name="producerMicroservice">The producer <see cref="Microservice" />.</param>
        /// <param name="producerTenant">The producer <see cref="TenantId" />.</param>
        public EventHorizon(Microservice consumerMicroservice, TenantId consumerTenant, Microservice producerMicroservice, TenantId producerTenant)
        {
            ConsumerMicroservice = consumerMicroservice;
            ConsumerTenant = consumerTenant;
            ProducerMicroservice = producerMicroservice;
            ProducerTenant = producerTenant;
        }

        /// <summary>
        /// Gets the consumer <see cref="Microservice" />.
        /// </summary>
        public Microservice ConsumerMicroservice { get; }

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
    }
}