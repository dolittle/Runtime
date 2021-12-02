// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Management;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Aggregates.Management;

/// <summary>
/// Represents an implementation of <see cref="ICanBindManagementServices"/> that exposes Aggregate Root management services.
/// </summary>
public class ManagementServices : ICanBindManagementServices
{
    readonly AggregateRootsService _aggregateRoots;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagementServices"/> class.
    /// </summary>
    /// <param name="aggregateRoots">The <see cref="AggregateRootsService"/>.</param>
    public ManagementServices(AggregateRootsService aggregateRoots)
    {
        _aggregateRoots = aggregateRoots;
    }

    /// <inheritdoc />
    public ServiceAspect Aspect => "Aggregates.Management";

    /// <inheritdoc />
    public IEnumerable<Service> BindServices() =>
        new[]
        {
            new Service(_aggregateRoots, Contracts.AggregateRoots.BindService(_aggregateRoots), Contracts.AggregateRoots.Descriptor),
        };
}