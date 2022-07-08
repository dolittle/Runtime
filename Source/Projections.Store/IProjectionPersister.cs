// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Store.Copies;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Defines a system for persisting Projection read models to the <see cref="IProjectionStore"/> and implementations of <see cref="IProjectionCopyStore"/>.
/// </summary>
public interface IProjectionPersister
{
    /// <summary>
    /// Try to replace a specific projection read model by projection definition and key.
    /// </summary>
    /// <param name="projection">The projection definition.</param>
    /// <param name="key">The projection key.</param>
    /// <param name="state">The new projection state.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the state was successfully replaced.</returns>
    Task<bool> TryReplace(ProjectionDefinition projection, ProjectionKey key, ProjectionState state, CancellationToken token);

    /// <summary>
    /// Try to remove a specific projection read model by projection definition and key.
    /// </summary>
    /// <param name="projection">The projection definition.</param>
    /// <param name="key">The projection key.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the state was successfully removed.</returns>
    Task<bool> TryRemove(ProjectionDefinition projection, ProjectionKey key, CancellationToken token);
    
    /// <summary>
    /// Try to drop all projection read models by projection definition.
    /// </summary>
    /// <param name="projection">The projection definition to drop.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the projection collection was successfully dropped.</returns>
    Task<bool> TryDrop(ProjectionDefinition projection, CancellationToken token); 
}
