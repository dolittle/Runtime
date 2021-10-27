// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Aggregates.Management
{
    /// <summary>
    /// Represents an Aggregate that is scoped to a specific Tenant.
    /// </summary>
    /// <param name="Tenant">The Tenant.</param>
    /// <param name="EventSource">The Event Source Id.</param>
    /// <param name="Version">The Aggregate Root Version.</param>
    public record TenantScopedAggregate(TenantId Tenant, EventSourceId EventSource, AggregateRootVersion Version);
}
