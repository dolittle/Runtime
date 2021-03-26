// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Defines a system that can compare a projection definition from a projection registration with the persisted definition.
    /// </summary>
    public interface ICompareProjectionDefinitionsForAllTenants
    {
        /// <summary>
        /// Checks whether the given <see cref="ProjectionDefinition" /> differs from the persisted definition.
        /// </summary>
        /// <param name="definition">The <see cref="ProjectionDefinition" /> to compare.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a value indicating whether the <see cref="ProjectionDefinition" /> differs from the persisted definition.</returns>
        Task<IDictionary<TenantId, ProjectionDefinitionComparisonResult>> DiffersFromPersisted(ProjectionDefinition definition, CancellationToken token);
    }
}
