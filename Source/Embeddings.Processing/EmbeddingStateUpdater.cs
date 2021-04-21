// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Projections.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IUpdateEmbeddingStates"/>.
    /// </summary>
    public class EmbeddingStateUpdater : IUpdateEmbeddingStates
    {
        readonly EmbeddingId _embedding;
        readonly IProjection _projection;
        readonly IEventStore _eventStore;
        readonly IEmbeddingStore _embeddingStore;
        readonly IConvertProjectionKeysToEventSourceIds _keyToEventSourceConverter;
        readonly ProjectionState _initialState;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingStateUpdater"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> that identifies the embedding.</param>
        /// <param name="projection">The <see cref="IProjection"/> that is used to update the state.</param>
        /// <param name="eventStore">The <see cref="IEventStore"/> that is used to fetch aggregate events.</param>
        /// <param name="embeddingStore">The <see cref="IEmbeddingStore"/> that is used to persist the states.</param>
        /// <param name="keyToEventSourceConverter">The <see cref="IConvertProjectionKeysToEventSourceIds"/> to use for converting projection keys to event source ids.</param>
        /// <param name="initialState">The <see cref="ProjectionState"/> that is used to initialize newly created states.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public EmbeddingStateUpdater(
            EmbeddingId embedding,
            IProjection projection,
            IEventStore eventStore,
            IEmbeddingStore embeddingStore,
            IConvertProjectionKeysToEventSourceIds keyToEventSourceConverter,
            ProjectionState initialState,
            ILogger logger)
        {
            _embedding = embedding;
            _projection = projection;
            _eventStore = eventStore;
            _embeddingStore = embeddingStore;
            _keyToEventSourceConverter = keyToEventSourceConverter;
            _initialState = initialState;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Try> TryUpdateAll(CancellationToken cancellationToken)
        {
            var keys = await _embeddingStore.TryGetKeys(_embedding, cancellationToken).ConfigureAwait(false);
            if (!keys.Success)
            {
                return keys;
            }

            var result = Try.Succeeded();
            foreach (var key in keys.Result)
            {
                var updateResult = await TryUpdateEmbedding(key, cancellationToken).ConfigureAwait(false);
                if (!updateResult.Success)
                {
                    _logger.LogWarning(
                        "Failed to update embedding state for embedding {Embedding} and key {Key}.{}",
                        _embedding,
                        key,
                        updateResult.HasException ?
                            $" {updateResult.Exception.Message}"
                            : string.Empty);
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

            return await TryProjectAll(currentState, unprocessedEvents, cancellationToken).ConfigureAwait(false);
            // return await TryHandleProjectionResult(projectionResult, key, unprocessedEvents, cancellationToken).ConfigureAwait(false);

        }

        async Task<Try<CommittedAggregateEvents>> TryGetUnprocessedEvents(ProjectionKey key, AggregateRootVersion aggregateRootVersion, CancellationToken cancellationToken)
        {
            try
            {
                var eventSource = _keyToEventSourceConverter.GetEventSourceIdFor(key);
                return await _eventStore.FetchForAggregateAfter(eventSource, _embedding.Value, aggregateRootVersion, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return ex;
            }

        }

        async Task<Try> TryProjectAll(ProjectionCurrentState currentState, CommittedAggregateEvents events, CancellationToken cancellationToken)
        {
            try
            {
                var result = Try.Succeeded();
                foreach (var @event in events)
                {
                    var projectionResult = await _projection.Project(currentState, @event, PartitionId.None, cancellationToken).ConfigureAwait(false);
                    if (projectionResult is ProjectionFailedResult failedResult)
                    {
                        return failedResult.Exception;
                    }
                    else if (projectionResult is ProjectionReplaceResult replaceResult)
                    {
                        currentState = new ProjectionCurrentState(ProjectionCurrentStateType.Persisted, replaceResult.State, currentState.Key);
                        result = await _embeddingStore.TryReplace(
                            _embedding,
                            currentState.Key,
                            @event.AggregateRootVersion.Value + 1,
                            replaceResult.State,
                            cancellationToken).ConfigureAwait(false);
                    }
                    else if (projectionResult is ProjectionDeleteResult deleteResult)
                    {
                        currentState = new ProjectionCurrentState(ProjectionCurrentStateType.CreatedFromInitialState, _initialState, currentState.Key);
                        result = await _embeddingStore.TryRemove(
                            _embedding,
                            currentState.Key,
                            @event.AggregateRootVersion.Value + 1,
                            cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        return new UnknownProjectionResultType(projectionResult);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}