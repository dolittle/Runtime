// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
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
            => TryProject(
                currentState,
                new UncommittedEvents(events.Select(_ => new UncommittedEvent(_.EventSource, _.Type, _.Public, _.Content)).ToList()),
                cancellationToken);

        /// <inheritdoc/>
        public async Task<Partial<EmbeddingCurrentState>> TryProject(EmbeddingCurrentState currentState, UncommittedEvents events, CancellationToken cancellationToken)
        {
            var i = 0;
            try
            {
                _logger.LogDebug("Trying to project {NumEvents} events on embedding {Embedding} and key {Key}", events.Count, _identifier, currentState.Key);
                for (; i < events.Count; i++)
                {
                    var tryProject = await TryProjectOne(currentState, events[i], cancellationToken).ConfigureAwait(false);
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
            catch (Exception ex)
            {
                return i == 0
                    ? ex
                    : Partial<EmbeddingCurrentState>.PartialSuccess(currentState, ex);
            }
        }

        async Task<Try<EmbeddingCurrentState>> TryProjectOne(EmbeddingCurrentState currentState, UncommittedEvent @event, CancellationToken cancellationToken)
        {
            var result = await _embedding.Project(currentState, @event, cancellationToken).ConfigureAwait(false);
            var nextAggregateRootVersion = currentState.Version.Value + 1;
            switch (result)
            {
                case ProjectionFailedResult failedResult:
                    return failedResult.Exception;
                case ProjectionReplaceResult replaceResult:
                    return new EmbeddingCurrentState(nextAggregateRootVersion, EmbeddingCurrentStateType.Persisted, replaceResult.State, currentState.Key);
                case ProjectionDeleteResult:
                    return new EmbeddingCurrentState(nextAggregateRootVersion, EmbeddingCurrentStateType.Deleted, _initialState, currentState.Key);
                default:
                    return new UnknownProjectionResultType(result);
            }
        }
    }
}
