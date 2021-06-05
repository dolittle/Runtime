// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Defines an event horizon connection to a producer microservice Runtime.
    /// </summary>
    public interface IEventHorizonConnection
    {
        /// <summary>
        /// Connects to the producer Runtime, and returns the response.
        /// </summary>
        /// <param name="subscription">The subscription to request public events for.</param>
        /// <param name="publicEventsPosition">The position of the producer Runtimes public stream to start from, one greater than last event already received.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the connection attempt.</param>
        /// <returns>
        /// A task that, when resolved returns the <see cref="SubscriptionResponse"/> from the connection to the producer Runtime.
        /// If <see cref="SubscriptionResponse.Success"/> is true, the connection is started and <see cref="StartRecevingEventsInto(AsyncProducerConsumerQueue{StreamEvent}, CancellationToken)"/> should be called.
        /// Else, the connection failed and should it should not be used.
        /// </returns>
        Task<SubscriptionResponse> Connect(
            SubscriptionId subscription,
            StreamPosition publicEventsPosition,
            CancellationToken cancellationToken);

        /// <summary>
        /// Starts the handling events from the producer Runtime.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to close the connection.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>

        /// <summary>
        /// Starts receiving events over the connection and writes then to the provided queue.
        /// </summary>
        /// <param name="connectionToStreamProcessorQueue">The async queue to write the events for the stream processor into</param>
        /// <param name="cancellationToken">A cancellation token that can be used to close the connection.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StartReceivingEventsInto(
            AsyncProducerConsumerQueue<StreamEvent> connectionToStreamProcessorQueue,
            CancellationToken cancellationToken);
    }
}
