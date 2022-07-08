// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using RuntimeExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Projections.Store.Services;

/// <summary>
/// Defines the service that retrieves projections.
/// </summary>
public interface IProjectionsService
{
    /// <summary>
    /// Try to get a projection state.
    /// </summary>
    /// <param name="projection"><see cref="ProjectionId" /> to get state from.</param>
    /// <param name="scope"><see cref="ScopeId" /> of the projetions.</param>
    /// <param name="key"><see cref="ProjectionKey" /> of the state to get.</param>
    /// <param name="context"><see cref="RuntimeExecutionContext" /> the <see cref="UncommittedEvents" /> are committed in.</param>
    /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of the <see cref="ProjectionCurrentState" />.</returns>
    Task<Try<ProjectionCurrentState>> TryGetOne(ProjectionId projection, ScopeId scope, ProjectionKey key, RuntimeExecutionContext context, CancellationToken token);

    /// <summary>
    /// Try to get all the projection states.
    /// </summary>
    /// <param name="projection"><see cref="ProjectionId" /> to get state from.</param>
    /// <param name="scope"><see cref="ScopeId" /> of the projetions.</param>
    /// <param name="context"><see cref="RuntimeExecutionContext" /> the <see cref="UncommittedAggregateEvents" /> are committed in.</param>
    /// <param name="token"><see cref="CancellationToken" /> for cancelling the task.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of the <see cref="IEnumerable{T}" /> of <see cref="ProjectionCurrentState" />.</returns>
    Task<Try<IAsyncEnumerable<ProjectionCurrentState>>> TryGetAll(ProjectionId projection, ScopeId scope, RuntimeExecutionContext context, CancellationToken token);
}
