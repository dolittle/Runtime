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
    public class EventFromEventHorizonHasWrongConsumerTenant : InvalidEventFromEventHorizon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFromEventHorizonHasWrongConsumerTenant"/> class.
        /// </summary>
        /// <param name="event">The <see cref="EventHorizonEvent" />.</param>
        /// <param name="expectedConsumerTenant">The expected consumer <see cref="TenantId" />.</param>
        public EventFromEventHorizonHasWrongConsumerTenant(EventHorizonEvent @event, TenantId expectedConsumerTenant)
            : base(@event.Type.Id.To<ArtifactId>(), @event.ProducerMicroservice.To<Microservice>(), @event.ProducerTenant.To<TenantId>(), $"Expected the consumer tenant to be '{expectedConsumerTenant}' but was '{@event.ConsumerTenant.To<TenantId>()}'.")
        {
        }
    }
}