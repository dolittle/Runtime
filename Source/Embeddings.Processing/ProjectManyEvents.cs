// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="IProjectManyEvents" />.
/// </summary>
[DisableAutoRegistration]
public class ProjectManyEvents : IProjectManyEvents
{
    readonly EmbeddingId _identifier;
    readonly IEmbedding _embedding;
    readonly ProjectionState _initialState;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes an instance of the <see cref="ProjectManyEvents" /> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId"/> that identifies the embedding.</param>
    /// <param name="projection">The <see cref="IProjection"/> that is used to update the state.</param>
    /// <param name="initialState">The <see cref="ProjectionState"/> that is used to initialize newly created states.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public ProjectManyEvents(
        EmbeddingId identifier, 
        IEmbedding embedding, 
        ProjectionState initialState, 
        ILogger logger)
    {
        _identifier = identifier;
        _embedding = embedding;
        _initialState = initialState;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<Partial<EmbeddingCurrentState>> TryProject(EmbeddingCurrentState currentState, CommittedAggregateEvents events, ExecutionContext executionContext, CancellationToken cancellationToken)
        => TryProject(
            currentState,
            new UncommittedEvents(events.Select(_ => new UncommittedEvent(_.EventSource, _.Type, _.Public, _.Content)).ToList()),
            executionContext,
            cancellationToken);

    /// <inheritdoc/>
    public async Task<Partial<EmbeddingCurrentState>> TryProject(EmbeddingCurrentState currentState, UncommittedEvents events, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.ProjectingEventsOnEmbedding(_identifier, currentState.Key, events);
        for (var i = 0; i < events.Count; i++)
        {
            var tryProject = await TryProjectOne(currentState, events[i], executionContext, cancellationToken).ConfigureAwait(false);
            if (!tryProject.Success)
            {
                return i == 0
                    ? tryProject.Exception
                    : Partial<EmbeddingCurrentState>.PartialSuccess(currentState, tryProject.Exception);
            }
            currentState = tryProject.Result;
        }
        return currentState;
    }

    async Task<Try<EmbeddingCurrentState>> TryProjectOne(EmbeddingCurrentState currentState, UncommittedEvent @event, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _embedding.Project(currentState, @event, executionContext, cancellationToken).ConfigureAwait(false);
            var nextAggregateRootVersion = currentState.Version.Value + 1;
            return result switch
            {
                ProjectionFailedResult failedResult => failedResult.Exception,
                ProjectionReplaceResult replaceResult => new EmbeddingCurrentState(nextAggregateRootVersion, EmbeddingCurrentStateType.Persisted, replaceResult.State, currentState.Key),
                ProjectionDeleteResult => new EmbeddingCurrentState(nextAggregateRootVersion, EmbeddingCurrentStateType.Deleted, _initialState, currentState.Key),
                _ => new UnknownProjectionResultType(result)
            };
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
