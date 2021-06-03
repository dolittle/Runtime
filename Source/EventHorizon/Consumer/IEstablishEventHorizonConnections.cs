// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system that can establish event horizon connections.
    /// </summary>
    public interface IEstablishEventHorizonConnections
    {
        /// <summary>
        /// Try to establish an event horizon connection.
        /// </summary>
        /// <param name="subscription">The <see cref="SubscriptionId" /> that this connection is for.</param>
        /// <param name="connectionAddress">The <see cref="MicroserviceAddress" /> to connect to.</param>
        /// <param name="eventsFromEventHorizon">The <see cref="AsyncProducerConsumerQueue{T}" /> that is used to transfer events from the event horizon connection to the stream processor.</param>
        /// <returns></returns>
        IEventHorizonConnection Establish(SubscriptionId subscription, MicroserviceAddress connectionAddress, AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon);
    }
}
