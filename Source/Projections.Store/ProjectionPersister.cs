// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.Copies;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Represents an implementation of <see cref="IProjectionPersister"/>.
/// </summary>
[SingletonPerTenant]
public class ProjectionPersister : IProjectionPersister
{
    readonly IProjectionStore _projectionStore;
    readonly IEnumerable<IProjectionCopyStore> _copyStores;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionPersister"/> class.
    /// </summary>
    /// <param name="projectionStore">The projection store to persist to.</param>
    /// <param name="copyStores">The copy stores to persist to.</param>
    /// <param name="logger">The logger to use.</param>
    public ProjectionPersister(IProjectionStore projectionStore, IEnumerable<IProjectionCopyStore> copyStores, ILogger logger)
    {
        _projectionStore = projectionStore;
        _copyStores = copyStores;
        _logger = logger;
    }

    public Task<bool> TryReplace(ProjectionDefinition projection, ProjectionKey key, ProjectionState state, CancellationToken token) => throw new System.NotImplementedException();

    public Task<bool> TryRemove(ProjectionDefinition projection, ProjectionKey key, CancellationToken token) => throw new System.NotImplementedException();

    public Task<bool> TryDrop(ProjectionDefinition projection, CancellationToken token) => throw new System.NotImplementedException();
}
