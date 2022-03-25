// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Defines a system that knows about projections.
/// </summary>
public interface IProjections
{
    /// <summary>
    /// Gets the information for all currently registered Projections.
    /// </summary>
    IEnumerable<ProjectionInfo> All { get; }

    /// <summary>
    /// Gets the current state of a Projection for all tenants.
    /// </summary>
    /// <param name="scopeId">The scope of the Projection to get the state for.</param>
    /// <param name="projectionId">The id of the Projection to get the state for.</param>
    /// <returns>The current states of the Projection.</returns>
    Try<IDictionary<TenantId, IStreamProcessorState>> CurrentStateFor(ScopeId scopeId, ProjectionId projectionId);

    /// <summary>
    /// Registers a Projection for all tenants.
    /// </summary>
    /// <param name="projection">The <see cref="IProjection"/> to start.</param>
    /// <param name="executionContext">The execution context to use for the projection processor.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="Try"/> of the registered <see cref="ProjectionProcessor"/>.</returns>
    Task<Try<ProjectionProcessor>> Register(IProjection projection, ExecutionContext executionContext, CancellationToken cancellationToken);

    /// <summary>
    /// Rebuilds Projection all read models for a specified Projection by dropping the old states and reprocessing all the events for a specific tenant.
    /// </summary>
    /// <param name="scopeId">The scope of the Projection to replay.</param>
    /// <param name="projectionId">The id of the Projection to replay.</param>
    /// <param name="tenantId">The tenant id to replay the Projection for.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="Try"/> result indicating the result of the operation.</returns>
    Task<Try> ReplayEventsForTenant(ScopeId scopeId, ProjectionId projectionId, TenantId tenantId);
    
    /// <summary>
    /// Rebuilds Projection all read models for a specified Projection by dropping the old states and reprocessing all the events for all tenants.
    /// </summary>
    /// <param name="scopeId">The scope of the Projection to replay.</param>
    /// <param name="projectionId">The id of the Projection to replay.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns a <see cref="Try"/> result indicating the result of the operation.</returns>
    Task<Try> ReplayEventsForAllTenants(ScopeId scopeId, ProjectionId projectionId);
}
