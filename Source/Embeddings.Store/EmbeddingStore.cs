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
                _logger.GettingOneEmbedding(embedding, key);

                var state = await _embeddingStates.TryGet(embedding, key, token);

                return state switch
                {
                    { Success: true } => new EmbeddingCurrentState(
                        state.Result.Version,
                        state.Result.IsRemoved ? EmbeddingCurrentStateType.Deleted : EmbeddingCurrentStateType.Persisted,
                        state.Result.State,
                        key),
                    { Success: false, Exception: EmbeddingStateDoesNotExist } => await TryGetInitialState(embedding, key, token),
                    _ => state.Exception
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting embedding {Embedding} and key {Key}", embedding, key);
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try<IEnumerable<EmbeddingCurrentState>>> TryGetAll(EmbeddingId embedding, CancellationToken token)
        {
            try
            {
                _logger.GettingAllEmbeddings(embedding);

                var tryGetStateTuples = await _embeddingStates.TryGetAll(embedding, token).ConfigureAwait(false);
                if (!tryGetStateTuples.Success)
                {
                    return tryGetStateTuples.Exception;
                }
                return tryGetStateTuples
                    .Select(_ =>
                         _.Select(resultTuple =>
                            new EmbeddingCurrentState(
                                resultTuple.State.Version,
                                EmbeddingCurrentStateType.Persisted,
                                resultTuple.State.State,
                                resultTuple.Key)));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting all readmodels of embedding {Embedding}", embedding);
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, CancellationToken token)
        {
            try
            {
                _logger.GettingEmbeddingKeys(embedding);

                var tryGetStateTuples = await _embeddingStates.TryGetAll(embedding, token).ConfigureAwait(false);
                if (!tryGetStateTuples.Success)
                {
                    return tryGetStateTuples.Exception;
                }
                return tryGetStateTuples
                    .Select(_ =>
                         _.Select(resultTuple => resultTuple.Key));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting keys for embedding {Embedding}", embedding);
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try> TryRemove(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, CancellationToken token)
        {
            try
            {
                _logger.RemovingEmbedding(embedding, key, version);

                var tryMarkAsRemoved = await _embeddingStates.TryMarkAsRemove(embedding, key, version, token).ConfigureAwait(false);
                return tryMarkAsRemoved.Success ? Try.Succeeded() : tryMarkAsRemoved.Exception;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Error removing embedding {Embedding} with key {Key} and version {Version}",
                    embedding,
                    key,
                    version);
                return ex;
            }
        }

        /// <inheritdoc/>
        public async Task<Try> TryReplace(
            EmbeddingId embedding,
            ProjectionKey key,
            AggregateRootVersion version,
            ProjectionState state,
            CancellationToken token)
        {
            try
            {
                _logger.ReplacingEmbedding(embedding, key, version, state);
                var embeddingState = new EmbeddingState(state, version, false);

                var tryReplace = await _embeddingStates.TryReplace(embedding, key, embeddingState, token)
                    .ConfigureAwait(false);
                return tryReplace.Success ? Try.Succeeded() : tryReplace.Exception;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Error replacing embedding {Embedding}, key {Key} and version {Version} with state {State}",
                    embedding,
                    key,
                    version,
                    state);
                return ex;
            }
        }

        async Task<Try<EmbeddingCurrentState>> TryGetInitialState(
            EmbeddingId embedding,
            ProjectionKey key,
            CancellationToken token)
        {
            var definition = await _embeddingDefinitions.TryGet(embedding, token).ConfigureAwait(false);
            if (!definition.Success)
            {
                return definition.Exception;
            }
            return new EmbeddingCurrentState(
                AggregateRootVersion.Initial,
                EmbeddingCurrentStateType.CreatedFromInitialState,
                definition.Result.InititalState,
                key);
        }
    }
}
