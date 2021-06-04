// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system that can get <see cref="IEventHorizonProcessor" />.
    /// </summary>
    public interface IEventHorizons
    {
        /// <summary>
        /// Gets an <see cref="IEventHorizonProcessor" /> for a <see cref="SubscriptionId" />.
        /// </summary>
        /// <param name="establishConnection"></param>
        /// <param name="subscription"></param>
        /// <param name="eventsFromEventHorizon"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IEventHorizonProcessor Get(
            MicroserviceAddress connectionAddress,
            SubscriptionId subscription,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon,
            CancellationToken cancellationToken);
    }
}
