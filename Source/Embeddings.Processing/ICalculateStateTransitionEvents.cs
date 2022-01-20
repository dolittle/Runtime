// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Defines a system that can calculate the events that needs to be committed in order to do a state transition from a current <see cref="EmbeddingCurrentState" /> to a desired <see cref="ProjectionState" />.
/// </summary>
public interface ICalculateStateTransitionEvents
{
    /// <summary>
    /// Try to calculate the events necessary to transition from the current state to a desired state.
    /// </summary>
    /// <param name="current">The <see cref="EmbeddingCurrentState"/> to transition from.</param>
    /// <param name="desired">The <see cref="ProjectionState"/> to transition to.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="UncommittedAggregateEvents" />.</returns>
    Task<Try<UncommittedAggregateEvents>> TryConverge(EmbeddingCurrentState current, ProjectionState desired, CancellationToken cancellationToken);

    /// <summary>
    /// Try to calculate the events necessary to delete the current state.
    /// </summary>
    /// <param name="current">The <see cref="EmbeddingCurrentState"/> to delete.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="UncommittedAggregateEvents" />.</returns>
    Task<Try<UncommittedAggregateEvents>> TryDelete(EmbeddingCurrentState current, CancellationToken cancellationToken);
}