// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Represents the implementation of <see cref="IEmbeddingStore"/>
    /// </summary>
    public class EmbeddingStore : IEmbeddingStore
    {

        readonly IEmbeddingStates _embeddingStates;
        readonly IEmbeddingDefinitions _embeddingDefinitions;
        readonly ILogger _logger;

        public EmbeddingStore(
            IEmbeddingStates embeddingStates,
            IEmbeddingDefinitions embeddingDefinitions,
            ILogger logger)
        {
            _embeddingStates = embeddingStates;
            _embeddingDefinitions = embeddingDefinitions;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Try<EmbeddingCurrentState>> TryGet(EmbeddingId embedding, ProjectionKey key, CancellationToken token)
        {
            try
            {
                // _logger.GettingOneEmbedding(embedding, key);

                var tryGetState = await _embeddingStates.TryGet(embedding, key, token);
                if (tryGetState.Success)
                {
                    return new EmbeddingCurrentState(
                        tryGetState.Result.Version,
                        ProjectionCurrentStateType.Persisted,
                        tryGetState.Result.State,
                        key);
                }
                if (tryGetState.HasException)
                {
                    return tryGetState.Exception;
                }

                var tryGetDefinition = await _embeddingDefinitions.TryGet(embedding, token).ConfigureAwait(false);
                if (tryGetDefinition.Success)
                {
                    return new EmbeddingCurrentState(
                        AggregateRootVersion.Initial,
                        ProjectionCurrentStateType.CreatedFromInitialState,
                        tryGetDefinition.Result.InititalState,
                        key);
                }
                if (tryGetDefinition.HasException)
                {
                    return tryGetDefinition.Exception;
                }

                return new FailedToGetEmbeddingState(embedding, key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error getting embedding with id {embedding} and key {key}");
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try<IEnumerable<EmbeddingCurrentState>>> TryGetAll(EmbeddingId embedding, CancellationToken token)
        {
            try
            {
                // _logger.GettingAllEmbeddings(embedding);

                var tryGetStateTuples = await _embeddingStates.TryGetAll(embedding, token).ConfigureAwait(false);
                if (tryGetStateTuples.Success)
                {
                    return tryGetStateTuples
                        .Select(_ =>
                             _.Select(resultTuple =>
                                new EmbeddingCurrentState(
                                    resultTuple.State.Version,
                                    ProjectionCurrentStateType.Persisted,
                                    resultTuple.State.State,
                                    resultTuple.Key)));
                }
                if (tryGetStateTuples.HasException)
                {
                    return tryGetStateTuples.Exception;
                }

                return new FailedToGetEmbeddingState(embedding);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error getting all embeddings with id {embedding}");
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, CancellationToken token)
        {
            try
            {
                // _logger.GettingEmbeddingKeys(embedding);

                var tryGetStateTuples = await _embeddingStates.TryGetAll(embedding, token).ConfigureAwait(false);
                if (tryGetStateTuples.Success)
                {
                    return tryGetStateTuples
                        .Select(_ =>
                             _.Select(resultTuple => resultTuple.Key));
                }
                if (tryGetStateTuples.HasException)
                {
                    return tryGetStateTuples.Exception;
                }

                return new FailedToGetEmbeddingKeys(embedding);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error getting an embedding's {embedding} keys");
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try> TryRemove(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, CancellationToken token)
        {
            try
            {
                // _logger.RemovingEmbedding(embedding);

                var tryMarkAsRemoved = await _embeddingStates.TryMarkAsRemove(embedding, key, version, token).ConfigureAwait(false);
                if (tryMarkAsRemoved.Success)
                {
                    return Try.Succeeded();
                }
                if (tryMarkAsRemoved.HasException)
                {
                    return tryMarkAsRemoved.Exception;
                }

                return new FailedToRemoveEmbedding(embedding, key, version);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error removing embedding with id {embedding}, key {key} and version {version}");
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try> TryReplace(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, ProjectionState state, CancellationToken token)
        {
            try
            {
                // _logger.ReplacingEmbedding(embedding);
                var embeddingState = new EmbeddingState(state, version);

                var tryReplace = await _embeddingStates.TryReplace(embedding, key, embeddingState, token).ConfigureAwait(false);
                if (tryReplace.Success)
                {
                    return Try.Succeeded();
                }
                if (tryReplace.HasException)
                {
                    return tryReplace.Exception;
                }

                return new FailedToReplaceEmbedding(embedding, key, version, state);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error replaceing embedding with id {embedding}, key {key} and version {version} with state {state}");
                return ex;
            }
        }
    }
}
