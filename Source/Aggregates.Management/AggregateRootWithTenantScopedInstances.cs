// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Aggregates.Management
{
    /// <summary>
    /// Represents an Aggregate Root with tenant-scoped Aggregates
    /// </summary>
    /// <param name="AggregateRoot">The Aggregate Root.</param>
    /// <param name="Aggregates">The Tenant Scoped Aggregates.</param>
    public record AggregateRootWithTenantScopedInstances(AggregateRoot AggregateRoot, IEnumerable<TenantScopedAggregateRootInstance> Aggregates);
}
