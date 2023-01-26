// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Projections.Store.Definition;

/// <summary>
/// Defines a repository for <see cref="ProjectionDefinition">projection definitions</see>.
/// </summary>
public interface IProjectionDefinitions
{
    /// <summary>
    /// Try to get a <see cref="ProjectionDefinition" /> for a projection.
    /// </summary>
    /// <param name="projection">The <see cref="ProjectionId" />.</param>
    /// <param name="scope">The <see cref="ScopeId" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="Try{TResult}" /> of <see cref="ProjectionDefinition" />.</returns>
    Task<Try<ProjectionDefinition>> TryGet(ProjectionId projection, ScopeId scope, CancellationToken token);

    /// <summary>
    /// Try to persist a <see cref="ProjectionDefinition" />.
    /// </summary>
    /// <param name="definition">The <see cref="ProjectionDefinition" />.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns a value indicating whether the new state was persisted.</returns>
    Task<bool> TryPersist(ProjectionDefinition definition, CancellationToken token);
}