// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;
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
        /// <param name="subscription">The <see cref="SubscriptionId" />.</param>
        /// <param name="eventsFromEventHorizon">The <see cref="AsyncProducerConsumerQueue{T}" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns></returns>
        Task<Try<SubscriptionResponse>> TryEstablish(
            SubscriptionId subscription,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon,
            CancellationToken cancellationToken);
    }
}
