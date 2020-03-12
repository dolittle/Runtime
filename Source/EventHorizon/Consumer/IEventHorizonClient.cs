// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.EventHorizon.Consumer
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
        /// <param name="subscriber">The subscriber <see cref="TenantId" />.</param>
        /// <param name="subscription">The <see cref="EventHorizonSubscription" />.</param>
        /// <returns>The task.</returns>
        Task StartSubscription(TenantId subscriber, EventHorizonSubscription subscription);
    }
}