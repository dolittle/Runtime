// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;

namespace Dolittle.Runtime.Server.Bootstrap;

/// <summary>
/// Defines a system that can perform a bootstrap procedure.
/// </summary>
public interface ICanPerformBoostrapProcedure
{
    /// <summary>
    /// Performs a bootstrap procedure.
    /// </summary>
    /// <returns>The <see cref="Task"/> representing the asynchronous action.</returns>
    Task Perform();
    
    /// <summary>
    /// Performs a bootstrap procedure for a specific tenant. 
    /// </summary>
    /// <returns>The <see cref="Task"/> representing the asynchronous action.</returns>
    Task PerformForTenant(TenantId tenant);
}
