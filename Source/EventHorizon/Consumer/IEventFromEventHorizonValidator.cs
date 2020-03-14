// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.EventHorizon;
using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system that can validate an event coming from an event horizon.
    /// </summary>
    public interface IEventFromEventHorizonValidator
    {
        /// <summary>
        /// Validates the <see cref="EventHorizonEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="EventHorizonEvent" /> from an event horizon.</param>
        /// <param name="expectedConsumerMicroservice">The expected consumer <see cref="Microservice" />.</param>
        /// <param name="expectedConsumerTenant">The expected consumer <see cref="TenantId" />.</param>
        /// <param name="expectedProducerMicroservice">The expected producer <see cref="Microservice" />.</param>
        /// <param name="expectedProducerTenant">The expected producer <see cref="TenantId" />.</param>
        void Validate(EventHorizonEvent @event, Microservice expectedConsumerMicroservice, TenantId expectedConsumerTenant, Microservice expectedProducerMicroservice, TenantId expectedProducerTenant);
    }
}