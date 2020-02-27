// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Defines a system that knows about subscribers on the event horizon.
    /// </summary>
    public interface IEventHorizonSubscribers
    {
        /// <summary>
        /// Subscribes a <see cref="TenantId" /> in a <see cref="Microservice" /> to the public events.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <param name="subscriber">The <see cref="TenantId" /> that is subscribing.</param>
        /// <param name="producer">The <see cref="TenantId" /> that is subscribed to.</param>
        /// <param name="publicEventsPosition">The <see cref="StreamPosition" /> in the public events stream to start distributing events from.</param>
        void Subscribe(Microservice microservice, TenantId subscriber, TenantId producer, StreamPosition publicEventsPosition);

        /// <summary>
        /// Unsubscribes a <see cref="TenantId" /> in a <see cref="Microservice" /> from the public events.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <param name="subscriber">The <see cref="TenantId" /> that is subscribing.</param>
        /// <param name="producer">The <see cref="TenantId" /> that is subscribed to.</param>
        void UnSubscribe(Microservice microservice, TenantId subscriber, TenantId producer);
    }
}