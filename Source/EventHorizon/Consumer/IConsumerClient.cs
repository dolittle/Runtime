// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system for handling inncomming public events from other microservices.
    /// </summary>
    public interface IConsumerClient
    {
        /// <summary>
        /// Asks the producer microservice to acknowledge the consent.
        /// </summary>
        /// <param name="eventHorizon">The <see cref="EventHorizon" />.</param>
        /// <param name="microserviceAddress">The <see cref="MicroserviceAddress" /> of the microservice to connect to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        void AcknowledgeConsent(EventHorizon eventHorizon, MicroserviceAddress microserviceAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts a subscription.
        /// </summary>
        /// <param name="eventHorizon">The <see cref="EventHorizon" />.</param>
        /// <param name="microserviceAddress">The <see cref="MicroserviceAddress" /> of the microservice to connect to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task.</returns>
        Task SubscribeTo(EventHorizon eventHorizon, MicroserviceAddress microserviceAddress, CancellationToken cancellationToken = default);
    }
}