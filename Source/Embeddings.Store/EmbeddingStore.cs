// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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

                var tryGetDefinition = await _embeddingDefinitions.TryGet(embedding, token);
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
                _logger.LogWarning(ex, "Error getting embedding");
                return ex;
            }
        }
        public Task<Try<IEnumerable<EmbeddingCurrentState>>> TryGetAll(EmbeddingId embedding, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        public Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        public Task<Try> TryRemove(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        public Task<Try> TryReplace(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, ProjectionState state, CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}
