// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a resilient subscription to an event horizon.
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Gets the <see cref="SubscriptionId" />.
        /// </summary>
        SubscriptionId Identifier { get; }

        /// <summary>
        /// Gets the current state of the subscription.
        /// </summary>
        SubscriptionState State { get; }

        /// <summary>
        /// Starts the resilient subscription.
        /// </summary>
        /// <remarks>
        /// This will start a new connection to the producer runtime, and will keep retrying the connection forever if it fails.
        /// </remarks>
        void Start();

        /// <summary>
        /// Gets a task that, when resolved returns the subscription response of the last connection.
        /// </summary>
        /// <remarks>
        /// If the subscription state is <see cref="SubscriptionState.Connected"/>, this will immediately resolve to the last connection response.
        /// Otherwise, it will resolve once the next connection attempt succeeds or fails.
        /// </remarks>
        Task<SubscriptionResponse> ConnectionResponse { get; }
    }
}
