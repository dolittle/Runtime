// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Projections.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="IUpdateEmbeddingStates"/>.
/// </summary>
public class EmbeddingStateUpdater : IUpdateEmbeddingStates
{
    readonly EmbeddingId _embedding;
    readonly IEventStore _eventStore;
    readonly IEmbeddingStore _embeddingStore;
    readonly IProjectManyEvents _projectManyEvents;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingStateUpdater"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId"/> that identifies the embedding.</param>
    /// <param name="eventStore">The <see cref="IEventStore"/> that is used to fetch aggregate events.</param>
    /// <param name="embeddingStore">The <see cref="IEmbeddingStore"/> that is used to persist the states.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public EmbeddingStateUpdater(
        EmbeddingId embedding,
        IEventStore eventStore,
        IEmbeddingStore embeddingStore,
        IProjectManyEvents projectManyEvents,
        ILogger logger)
    {
        _embedding = embedding;
        _eventStore = eventStore;
        _embeddingStore = embeddingStore;
        _projectManyEvents = projectManyEvents;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Try> TryUpdateAll(CancellationToken cancellationToken)
    {
        _logger.UpdatingAllEmbeddingStates(_embedding);
        var keys = await _embeddingStore.TryGetKeys(_embedding, cancellationToken).ConfigureAwait(false);
        if (!keys.Success)
        {
            return keys;
        }

        var result = Try.Succeeded();
        foreach (var key in keys.Result)
        {
            _logger.UpdatingEmbeddingStateFor(_embedding, key);
            var updateResult = await TryUpdateEmbedding(key, cancellationToken).ConfigureAwait(false);
            if (!updateResult.Success)
            {
                _logger.FailedUpdatingEmbeddingStateFor(_embedding, key, updateResult.Exception);
                result = result.Success ? updateResult : result;
            }
        }

        return result;
    }

    async Task<Try> TryUpdateEmbedding(ProjectionKey key, CancellationToken cancellationToken)
    {
        var currentState = await _embeddingStore.TryGet(_embedding, key, cancellationToken).ConfigureAwait(false);
        if (!currentState.Success)
        {
            return currentState;
        }
        var unprocessedEvents = await TryGetUnprocessedEvents(key, currentState.Result.Version, cancellationToken).ConfigureAwait(false);
        if (!unprocessedEvents.Success)
        {
            return Try.Failed(unprocessedEvents.Exception);
        }
        if (!unprocessedEvents.Result.Any())
        {
            return Try.Succeeded();
        }

        var projectedState = await _projectManyEvents.TryProject(currentState, unprocessedEvents, cancellationToken).ConfigureAwait(false);
        if (projectedState.Success || projectedState.IsPartialResult)
        {
            var updateState = await TryUpdateOrDeleteState(projectedState.Result, cancellationToken).ConfigureAwait(false);
            if (updateState.Success && projectedState.IsPartialResult)
            {
                return projectedState.Exception;
            }

            return updateState;
        }

        return Try.Failed(projectedState.Exception);
    }

    Task<Try> TryUpdateOrDeleteState(EmbeddingCurrentState state, CancellationToken cancellationToken)
        => state.Type == EmbeddingCurrentStateType.Deleted
            ? _embeddingStore.TryRemove(
                _embedding,
                state.Key,
                state.Version,
                cancellationToken)
            : _embeddingStore.TryReplace(
                _embedding,
                state.Key,
                state.Version,
                state.State,
                cancellationToken);

    async Task<Try<CommittedAggregateEvents>> TryGetUnprocessedEvents(ProjectionKey key, AggregateRootVersion aggregateRootVersion, CancellationToken cancellationToken)
    {
        try
        {
            return await _eventStore.FetchForAggregateAfter(
                key.Value,
                _embedding.Value,
                aggregateRootVersion,
                cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}