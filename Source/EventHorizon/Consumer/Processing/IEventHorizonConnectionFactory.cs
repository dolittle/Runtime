// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system that can create instances of <see cref="IEventHorizonConnection"/>.
    /// </summary>
    public interface IEventHorizonConnectionFactory
    {
        /// <summary>
        /// Creates a new <see cref="IEventHorizonConnection"/> to another Runtime for a subscription.
        /// </summary>
        /// <param name="connectionAddress">The address of the other microservices Runtime to connect to.</param>
        /// <param name="subscription">The subscription to request public events for.</param>
        /// <returns>A new <see cref="IEventHorizonConnection"/> that is ready to be connected.</returns>
        IEventHorizonConnection Create(MicroserviceAddress connectionAddress, SubscriptionId subscription);
    }
}
