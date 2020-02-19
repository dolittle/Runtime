// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Defines a system for handling inncomming public events from other microservices.
    /// </summary>
    public interface IEventHorizonClient
    {
        /// <summary>
        /// Subscribes based on event-horizon config.
        /// </summary>
        void Subscribe();

        /// <summary>
        /// Starts a subscription.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <param name="producer">The producer <see cref="TenantId" />.</param>
        /// <param name="subscriber">The subscriber <see cref="TenantId" />.</param>
        /// <returns>The task.</returns>
        Task StartSubscription(Microservice microservice, TenantId producer, TenantId subscriber);
    }
}