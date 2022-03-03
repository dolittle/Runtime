// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates.Management;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates;

/// <summary>
/// Defines the AggregateRoots management client.
/// </summary>
public interface IManagementClient
{
    /// <summary>
    /// Gets all registered Aggregate Roots or for a specific Tenant if specified.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="tenant">The Tenant to get Aggregate Roots for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<IEnumerable<AggregateRootWithTenantScopedInstances>> GetAll(MicroserviceAddress runtime, TenantId tenant = null);

    /// <summary>
    /// Gets the registered Aggregate Root with the given identifier and for a specific Tenant if specified.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="aggregateRootId">The Aggregate Root identifier.</param>
    /// <param name="tenant">The Tenant to get Aggregate Root for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Try<AggregateRootWithTenantScopedInstances>> Get(MicroserviceAddress runtime, ArtifactId aggregateRootId, TenantId tenant = null);

    /// <summary>
    /// Gets all committed aggregate events for an Aggregate Root Instance
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="aggregateRootId">The Aggregate Root identifier.</param>
    /// <param name="eventSourceId">The Event Source Id of the Aggregate Root Instance.</param>
    /// <param name="tenant">The Tenant to get the Committed Aggregate Events for.</param>
    /// <returns></returns>
    Task<CommittedAggregateEvents> GetEvents(MicroserviceAddress runtime, ArtifactId aggregateRootId, EventSourceId eventSourceId, TenantId tenant = null);

}
