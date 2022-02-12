// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Defines a projection.
/// </summary>
public interface IProjection
{
    /// <summary>
    /// Gets the <see cref="ProjectionDefinition"/> for this <see cref="IProjection"/>.
    /// </summary>
    ProjectionDefinition Definition { get; }
    
    /// <summary>
    /// Gets the alias of the Projection if set, or <see cref="ProjectionAlias.NotSet"/> if not passed from the Client.
    /// </summary>
    ProjectionAlias Alias { get; }
    
    /// <summary>
    /// Gets a value indicating whether or not the Client passed along an alias for the Projection.
    /// </summary>
    bool HasAlias { get;  }
    
    /// <summary>
    /// Project a <see cref="CommittedEvent" /> from a <see cref="PartitionId">partition</see> onto a <see cref="ProjectionCurrentState"/> to calculate the new <see cref="ProjectionState"/>.
    /// </summary>
    /// <param name="state">The <see cref="ProjectionCurrentState"/> to update.</param>
    /// <param name="event">The <see cref="CommittedEvent"/> to use to update the state.</param>
    /// <param name="partitionId">The <see cref="PartitionId"/> the event came from.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns an <see cref="IProjectionResult" />.</returns>
    Task<IProjectionResult> Project(ProjectionCurrentState state, CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken);

    /// <summary>
    /// Retry projecting a <see cref="CommittedEvent" /> from a <see cref="PartitionId">partition</see> onto a <see cref="ProjectionCurrentState"/> to calculate the new <see cref="ProjectionState"/>.
    /// </summary>
    /// <param name="state">The <see cref="ProjectionCurrentState"/> to update.</param>
    /// <param name="event">The <see cref="CommittedEvent"/> to use to update the state.</param>
    /// <param name="partitionId">The <see cref="PartitionId"/> the event came from.</param>
    /// <param name="failureReason">The reason the processor was failing.</param>
    /// <param name="retryCount">The retry count.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns an <see cref="IProjectionResult" />.</returns>
    Task<IProjectionResult> Project(ProjectionCurrentState state, CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken);
}
