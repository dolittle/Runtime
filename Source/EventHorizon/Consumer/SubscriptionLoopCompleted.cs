// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Exception that gets thrown when the processing loop for an event horizon subscription completes, which it never should.
/// </summary>
public class SubscriptionLoopCompleted : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="SubscriptionLoopCompleted"/> class.
    /// </summary>
    /// <param name="subscription">The identifier for the subscription that the processing loop failed for.</param>
    public SubscriptionLoopCompleted(SubscriptionId subscription)
        : base($"Subscription loop completed for subscription: tenant {subscription.ConsumerTenantId.Value} from partition {subscription.PartitionId.Value} in stream {subscription.StreamId.Value} from tenant {subscription.ProducerTenantId.Value} in microservice {subscription.ProducerMicroserviceId.Value} into scope {subscription.ScopeId.Value}")
    {
    }
}