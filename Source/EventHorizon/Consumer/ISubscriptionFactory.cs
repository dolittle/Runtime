// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Microservices;

namespace Dolittle.Runtime.EventHorizon.Consumer;

/// <summary>
/// Defines a factory that can create <see cref="ISubscription" />.
/// </summary>
public interface ISubscriptionFactory
{
    /// <summary>
    /// Creates an <see cref="ISubscription" />.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="producerMicroserviceAddress">The producer microservice address.</param>
    /// <param name="executionContext">The <see cref="ExecutionContext"/>.</param>
    /// <returns>The created <see cref="ISubscription" />.</returns>
    ISubscription Create(SubscriptionId subscriptionId, MicroserviceAddress producerMicroserviceAddress, ExecutionContext executionContext);
}
