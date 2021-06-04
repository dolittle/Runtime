// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Defines a system that connects to the producer Runtime.
    /// </summary>
    public interface IEventHorizonProcessor
    {
        /// <summary>
        /// Connects to the event horizon.
        /// </summary>
        /// <param name="publicEventsPosition"></param>
        /// <returns></returns>
        Task<SubscriptionResponse> Connect(StreamPosition publicEventsPosition);

        /// <summary>
        /// Starts handling events from the producer microservice.
        /// </summary>
        /// <returns></returns>
        Task StartHandleEvents();
    }
}
