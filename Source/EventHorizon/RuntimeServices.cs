// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.EventHorizon;

/// <summary>
/// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
/// runtime service implementations for Heads.
/// </summary>
public class RuntimeServices : ICanBindRuntimeServices
{
    readonly SubscriptionsService _subscriptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
    /// </summary>
    /// <param name="subscriptions">The <see cref="SubscriptionsService" />.</param>
    public RuntimeServices(SubscriptionsService subscriptions) => _subscriptions = subscriptions;

    /// <inheritdoc/>
    public ServiceAspect Aspect => "EventHorizon";

    /// <inheritdoc/>
    public IEnumerable<Service> BindServices() =>
        new Service[]
        {
            new(_subscriptions, Contracts.Subscriptions.BindService(_subscriptions), Contracts.Subscriptions.Descriptor)
        };
}