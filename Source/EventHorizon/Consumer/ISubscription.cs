// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Store.EventHorizon;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Defines a resilient subscription to an event horizon.
    /// </summary>
    public interface ISubscription : IDisposable
    {
        /// <summary>
        /// Registers the subscription.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> that, when resolved, returns a <see cref="SubscriptionResponse" />.</returns>
        Task<SubscriptionResponse> Register();

        /// <summary>
        /// Gets the <see cref="SubscriptionId" />.
        /// </summary>
        SubscriptionId Identifier { get; }

        /// <summary>
        /// Gets the <see cref="ConsentId" />.
        /// </summary>
        ConsentId Consent { get; }
    }
}
