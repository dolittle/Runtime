// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Events.Store.Streams;
using Grpc.Core;
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
            Func<AsyncDuplexStreamingCall<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage>> establishConnection,
            SubscriptionId subscription,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon,
            CancellationToken cancellationToken);
    }
}
