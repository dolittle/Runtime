// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectManyEvents" />.
    /// </summary>
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
        public ProjectManyEvents(EmbeddingId identifier, IEmbedding embedding, ProjectionState initialState, ILogger logger)
        {
            _identifier = identifier;
            _embedding = embedding;
            _initialState = initialState;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<Partial<EmbeddingCurrentState>> TryProject(EmbeddingCurrentState currentState, CommittedAggregateEvents events, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            // try
            // {
            //     _logger.LogDebug("Trying to projection {NumEvents} events on embedding {Embedding} and key {Key}", events.Count, _embedding, currentState.Key);
            //     foreach (var @event in events)
            //     {
            //         var projectionResult = await _projection.Project(currentState, @event, PartitionId.None, cancellationToken).ConfigureAwait(false);
            //         if (!TryHandleProjectionResult(projectionResult, @event.AggregateRootVersion, currentState.Key, out var projectedState, out var error))
            //         {
            //             return currentState == null ? ;
            //         }
            //     }
            //     return currentState;
            // }
            // catch (Exception ex)
            // {
            //     return ex;
            // }
        }

        /// <inheritdoc/>
        public Task<Partial<EmbeddingCurrentState>> TryProject(EmbeddingCurrentState currentState, UncommittedEvents events, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // bool TryHandleProjectionResult(IProjectionResult result, AggregateRootVersion aggregateRootVersion, ProjectionKey key, out EmbeddingCurrentState currentState, out Exception error)
        // {
        //     currentState = null;
        //     error = null;
        //     switch (result)
        //     {
        //         case ProjectionFailedResult failedResult:
        //             error = failedResult.Exception;
        //             return false;
        //         case ProjectionReplaceResult replaceResult:
        //             currentState = new EmbeddingCurrentState(aggregateRootVersion, EmbeddingCurrentStateType.Persisted, replaceResult.State, key);
        //             return true;
        //         case ProjectionDeleteResult:
        //             currentState = new EmbeddingCurrentState(aggregateRootVersion, EmbeddingCurrentStateType.Deleted, _initialState, key);
        //             return true;
        //         default:
        //             error = new UnknownProjectionResultType(result);
        //             return false;

        //     }
        // }
    }
}