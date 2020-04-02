// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system for handling inncomming public events from other microservices.
    /// </summary>
    public interface IConsumerClient
    {
        /// <summary>
        /// Starts a subscription.
        /// </summary>
        /// <param name="eventHorizon">The <see cref="EventHorizon" />.</param>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="publicStream">The public <see cref="StreamId" />.</param>
        /// <param name="partition">The <see cref="PartitionId" />.</param>
        /// <param name="microserviceAddress">The <see cref="MicroserviceAddress" /> of the microservice to connect to.</param>
        /// <returns>The task.</returns>
        Task SubscribeTo(EventHorizon eventHorizon, ScopeId scope, StreamId publicStream, PartitionId partition, MicroserviceAddress microserviceAddress);
    }
}