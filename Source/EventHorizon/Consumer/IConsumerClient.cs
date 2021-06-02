// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system for handling incoming public events from other microservices.
    /// </summary>
    public interface IConsumerClient : IDisposable
    {
        /// <summary>
        /// Handles a subscription request.
        /// </summary>
        /// <param name="subscriptionId">The <see cref="SubscriptionId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="SubscriptionResponse" />.</returns>
        Task<SubscriptionResponse> HandleSubscriptionRequest(SubscriptionId subscriptionId, CancellationToken cancellationToken);
    }
}
