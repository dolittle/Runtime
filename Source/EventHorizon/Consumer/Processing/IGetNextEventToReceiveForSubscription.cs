// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
{
    /// <summary>
    /// Defines a system that knows about the next events to receive for an event horion subscription.
    /// </summary>
    public interface IGetNextEventToReceiveForSubscription
    {
        /// <summary>
        /// Gets the stream position of the next public event to receive for the provided subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription to get next position for.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>A task that, when resolved returns the position of the next public event to receive.</returns>
        Task<StreamPosition> GetNextEventToReceiveFor(SubscriptionId subscriptionId, CancellationToken cancellationToken);
    }
}
