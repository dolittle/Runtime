// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// Defines the Projections management client.
/// </summary>
public interface IManagementClient
{
    /// <summary>
    /// Get the <see cref="ProjectionStatus"/> of all registered Projections.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="tenant">The Tenant to get Stream Processor states for, or null to get all.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="ProjectionStatus"/> of all registered Projections.</returns>
    Task<IEnumerable<ProjectionStatus>> GetAll(MicroserviceAddress runtime, TenantId tenant = null);

    /// <summary>
    /// Get the <see cref="ProjectionStatus"/> of a registered Projection by <see cref="ScopeId"/> and <see cref="ProjectionId"/>.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="scope">The scope of the Projection.</param>
    /// <param name="projection">The id of the Projection.</param>
    /// <param name="tenant">The Tenant to get Stream Processor states for, or null to get all.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="Try"/> containing the <see cref="ProjectionStatus"/>-</returns>
    Task<Try<ProjectionStatus>> Get(MicroserviceAddress runtime, ScopeId scope, ProjectionId projection, TenantId tenant = null);
    
    /// <summary>
    /// Replay a registered Projection by <see cref="ScopeId"/> and <see cref="ProjectionId"/>, clearing persisted read models and reprocess events.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="scope">The scope of the Projection.</param>
    /// <param name="projection">The id of the Projection.</param>
    /// <param name="tenant">The Tenant to replay the Projection for, or null to get all.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the <see cref="Try"/> result of the operation.</returns>
    Task<Try> Replay(MicroserviceAddress runtime, ScopeId scope, ProjectionId projection, TenantId tenant = null);
}
