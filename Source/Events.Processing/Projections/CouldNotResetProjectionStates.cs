// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Exception that gets throw when the projection states could not be reset for a tenant.
/// </summary>
public class CouldNotResetProjectionStates : Exception
{
    public CouldNotResetProjectionStates(ProjectionDefinition projectionDefinition, TenantId tenant)
        : base($"Could not reset projection states for projection {projectionDefinition.Projection.Value} in scope {projectionDefinition.Scope.Value} for tenant {tenant.Value}")
    {
    }
}