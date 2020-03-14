// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.EventHorizon;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Protobuf;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when an event from an event horizon does not have the correct consumer tenant.
    /// </summary>
    public class EventFromEventHorizonHasWrongProducerTenant : InvalidEventFromEventHorizon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFromEventHorizonHasWrongProducerTenant"/> class.
        /// </summary>
        /// <param name="event">The <see cref="EventHorizonEvent" />.</param>
        /// <param name="expectedProducerTenant">The expected producer <see cref="TenantId" />.</param>
        public EventFromEventHorizonHasWrongProducerTenant(EventHorizonEvent @event, TenantId expectedProducerTenant)
            : base(@event.Type.Id.To<ArtifactId>(), @event.ProducerMicroservice.To<Microservice>(), @event.ProducerTenant.To<TenantId>(), $"Expected the producer tenant to be '{expectedProducerTenant}' but was '{@event.ProducerTenant.To<TenantId>()}'.")
        {
        }
    }
}