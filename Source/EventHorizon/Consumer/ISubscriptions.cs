// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Defines a system that keeps track of event horizon subscriptions.
/// </summary>
public interface ISubscriptions
{
    /// <summary>
    /// Creates a new event horizon subscription if one does not already exist for the provided <see cref="SubscriptionId"/>.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier which describes the subscription to create.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
    /// <returns>
    /// A task that, when resolved returns the <see cref="SubscriptionResponse"/> from the connection to the producer Runtime.
    /// If the subscription is already connected, this immediately resolves to the last connection result.
    /// Otherwise, it will resolve once the next connection attempt succeeds or fails.
    /// </returns>
    Task<SubscriptionResponse> Subscribe(SubscriptionId subscriptionId, ExecutionContext executionContext);
}
