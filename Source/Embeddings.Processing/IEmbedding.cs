// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Defines an embedding.
/// </summary>
public interface IEmbedding
{
    /// <summary>
    /// Tries to get <see cref="UncommittedEvents"/> that will get the current state closer to the desired state.
    /// </summary>
    /// <param name="currentState">The <see cref="EmbeddingCurrentState">current state</see>.</param>
    /// <param name="desiredState">The <see cref="ProjectionState">desired state</see>.</param>
    /// <param name="executionContext">The execution context to execute the update operation in.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="UncommittedEvents" />.</returns>
    Task<Try<UncommittedEvents>> TryCompare(EmbeddingCurrentState currentState, ProjectionState desiredState, ExecutionContext executionContext, CancellationToken cancellationToken);

    /// <summary>
    /// Tries to get <see cref=" UncommittedEvents"/> that will get the current state closer to be deleted.
    /// </summary>
    /// <param name="currentState">The <see cref="EmbeddingCurrentState" />.</param>
    /// <param name="executionContext">The execution context to execute the delete operation in.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="UncommittedEvents" />.</returns>
    Task<Try<UncommittedEvents>> TryDelete(EmbeddingCurrentState currentState, ExecutionContext executionContext, CancellationToken cancellationToken);

    /// <summary>
    /// Projects a <see cref="UncommittedEvent" /> from onto a <see cref="ProjectionCurrentState"/> to calculate the new <see cref="ProjectionState"/>.
    /// </summary>
    /// <param name="state">The <see cref="ProjectionCurrentState"/> to update.</param>
    /// <param name="event">The <see cref="UncommittedEvent"/> to use to update the state.</param>
    /// <param name="executionContext">The execution context to execute the projection operation in.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns an <see cref="IProjectionResult" />.</returns>
    Task<IProjectionResult> Project(ProjectionCurrentState state, UncommittedEvent @event, ExecutionContext executionContext, CancellationToken cancellationToken);
}
