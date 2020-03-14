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
    /// Exception that gets thrown when an event from an event horizon does not have the correct consumer microservice.
    /// </summary>
    public class EventFromEventHorizonHasWrongProducerMicroservice : InvalidEventFromEventHorizon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFromEventHorizonHasWrongProducerMicroservice"/> class.
        /// </summary>
        /// <param name="event">The <see cref="EventHorizonEvent" />.</param>
        /// <param name="expectedProducerMicroservice">The expected producer <see cref="Microservice" />.</param>
        public EventFromEventHorizonHasWrongProducerMicroservice(EventHorizonEvent @event, Microservice expectedProducerMicroservice)
            : base(@event.Type.Id.To<ArtifactId>(), @event.ProducerMicroservice.To<Microservice>(), @event.ProducerTenant.To<TenantId>(), $"Expected the producer microservice to be '{expectedProducerMicroservice}' but was '{@event.ProducerMicroservice.To<Microservice>()}'.")
        {
        }
    }
}