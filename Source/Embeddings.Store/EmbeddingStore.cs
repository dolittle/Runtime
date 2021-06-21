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

        /// <summary>
        /// Initializes an instance of the <see cref="EmbeddingStore" /> class.
        /// </summary>
        /// <param name="embeddingStates">The embedding states.</param>
        /// <param name="embeddingDefinitions">The embedding definitions.</param>
        /// <param name="logger">The logger for logging messages.</param>
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
                    { Success: true, Result: { IsRemoved: true } } => await TryGetInitialState(
                        embedding,
                        key,
                        state.Result.Version,
                        token).ConfigureAwait(false),
                    { Success: true } => new EmbeddingCurrentState(
                        state.Result.Version,
                        EmbeddingCurrentStateType.Persisted,
                        state.Result.State,
                        key),
                    { Success: false, Exception: EmbeddingStateDoesNotExist } =>
                        await TryGetInitialState(embedding, key, token).ConfigureAwait(false),
                    _ => state.Exception
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting embedding {Embedding} and key {Key}", embedding.Value, key.Value);
                return ex;
            }
        }

        /// <inheritdoc/>
        public Task<Try<IEnumerable<EmbeddingCurrentState>>> TryGetAll(EmbeddingId embedding, CancellationToken token)
            => TryGetAll(embedding, false, token);


        /// <inheritdoc/>
        public async Task<Try<IEnumerable<EmbeddingCurrentState>>> TryGetAll(EmbeddingId embedding, bool includeRemoved, CancellationToken token)
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
                        _.Where(resultTuple => resultTuple.State.IsRemoved == includeRemoved)
                        .Select(resultTuple =>
                            new EmbeddingCurrentState(
                                resultTuple.State.Version,
                                resultTuple.State.IsRemoved ? EmbeddingCurrentStateType.Deleted : EmbeddingCurrentStateType.Persisted,
                                resultTuple.State.State,
                                resultTuple.Key)));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting all instances of embedding {Embedding}", embedding.Value);
                return ex;
            }
        }

        /// <inheritdoc/>
        public Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, CancellationToken token)
            => TryGetKeys(embedding, false, token);


        /// <inheritdoc/>
        public async Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, bool includeRemoved, CancellationToken token)
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
                        _.Where(resultTuple => resultTuple.State.IsRemoved == includeRemoved)
                        .Select(resultTuple => resultTuple.Key));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting keys for embedding {Embedding}", embedding.Value);
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
                    embedding.Value,
                    key.Value,
                    version.Value);
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

                var tryReplace = await _embeddingStates.TryReplace(embedding, key, embeddingState, token).ConfigureAwait(false);
                return tryReplace.Success ? Try.Succeeded() : tryReplace.Exception;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Error replacing embedding {Embedding}, key {Key} and version {Version} with state {State}",
                    embedding.Value,
                    key.Value,
                    version.Value,
                    state.Value);
                return ex;
            }
        }

        Task<Try<EmbeddingCurrentState>> TryGetInitialState(
            EmbeddingId embedding,
            ProjectionKey key,
            CancellationToken token)
            => TryGetInitialState(embedding, key, AggregateRootVersion.Initial, token);

        async Task<Try<EmbeddingCurrentState>> TryGetInitialState(
            EmbeddingId embedding,
            ProjectionKey key,
            AggregateRootVersion version,
            CancellationToken token)
        {
            var definition = await _embeddingDefinitions.TryGet(embedding, token).ConfigureAwait(false);
            if (!definition.Success)
            {
                return definition.Exception;
            }
            return new EmbeddingCurrentState(
                version,
                EmbeddingCurrentStateType.CreatedFromInitialState,
                definition.Result.InititalState,
                key);
        }
    }
}
