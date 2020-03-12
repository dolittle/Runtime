// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Runtime.Microservices;
using Dolittle.Tenancy;

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
        /// <param name="microservice">The <see cref="Microservice" /> to subscribe to.</param>
        /// <param name="tenant">The <see cref="TenantId" /> to subscribe to.</param>
        /// <param name="host">The <see cref="MicroserviceHost" /> of the microservice to connect to.</param>
        /// <param name="port">The <see cref="MicroservicePort" /> of the microservice to connect.</param>
        /// <returns>The task.</returns>
        Task SubscribeTo(Microservice microservice, TenantId tenant, MicroserviceHost host, MicroservicePort port);
    }
}