// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="ProjectionDefinition" /> could not be persisted for a tenant.
    /// </summary>
    public class CouldNotPersistProjectionDefinition : Exception
    {
        public CouldNotPersistProjectionDefinition(ProjectionDefinition projectionDefinition, TenantId tenant)
            : base($"Could not persist projection definition for projection {projectionDefinition.Projection.Value} in scope {projectionDefinition.Scope.Value} for tenant {tenant.Value}")
        {
        }
    }
}