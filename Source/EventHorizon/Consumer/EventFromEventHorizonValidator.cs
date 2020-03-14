// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using contracts::Dolittle.Runtime.EventHorizon;
using Dolittle.Applications;
using Dolittle.Protobuf;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventFromEventHorizonValidator" />.
    /// </summary>
    public class EventFromEventHorizonValidator : IEventFromEventHorizonValidator
    {
        /// <inheritdoc/>
        public void Validate(EventHorizonEvent @event, Microservice expectedConsumerMicroservice, TenantId expectedConsumerTenant, Microservice expectedProducerMicroservice, TenantId expectedProducerTenant)
        {
            ThrowIfWrongConsumerMicroservice(@event, expectedConsumerMicroservice);
            ThrowIfWrongConsumerTenant(@event, expectedConsumerTenant);
            ThrowIfWrongProducerMicroservice(@event, expectedProducerMicroservice);
            ThrowIfWrongProducerTenant(@event, expectedProducerTenant);
        }

        void ThrowIfWrongConsumerMicroservice(EventHorizonEvent @event, Microservice expectedConsumerMicroservice)
        {
            if (@event.ConsumerMicroservice.To<Microservice>() != expectedConsumerMicroservice) throw new EventFromEventHorizonHasWrongConsumerMicroservice(@event, expectedConsumerMicroservice);
        }

        void ThrowIfWrongConsumerTenant(EventHorizonEvent @event, TenantId expectedConsumerTenant)
        {
            if (@event.ConsumerTenant.To<TenantId>() != expectedConsumerTenant) throw new EventFromEventHorizonHasWrongConsumerTenant(@event, expectedConsumerTenant);
        }

        void ThrowIfWrongProducerMicroservice(EventHorizonEvent @event, Microservice expectedProducerMicroservice)
        {
            if (@event.ProducerMicroservice.To<Microservice>() != expectedProducerMicroservice) throw new EventFromEventHorizonHasWrongProducerMicroservice(@event, expectedProducerMicroservice);
        }

        void ThrowIfWrongProducerTenant(EventHorizonEvent @event, TenantId expectedProducerTenant)
        {
            if (@event.ProducerTenant.To<TenantId>() != expectedProducerTenant) throw new EventFromEventHorizonHasWrongProducerTenant(@event, expectedProducerTenant);
        }
    }
}