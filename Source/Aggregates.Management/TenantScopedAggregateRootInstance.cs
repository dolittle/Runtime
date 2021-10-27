// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Aggregates.Management
{
    /// <summary>
    /// Represents an Aggregate Root Instance that is scoped to a specific Tenant.
    /// </summary>
    /// <param name="Tenant">The Tenant.</param>
    /// <param name="Instance">The Aggregate Root Instance.</param>
    public record TenantScopedAggregateRootInstance(TenantId Tenant, AggregateRootInstance Instance);
}
