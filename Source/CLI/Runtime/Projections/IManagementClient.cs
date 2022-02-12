// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// Defines the Projections management client.
/// </summary>
public interface IManagementClient
{
    /// <summary>
    /// Gets all running Projections for a specific tenant if specified.
    /// </summary>
    /// <param name="runtime">The address of the Runtime to connect to.</param>
    /// <param name="tenant">The Tenant to get Event Handlers for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<IEnumerable<ProjectionStatus>> GetAll(MicroserviceAddress runtime, TenantId tenant = null);
}
