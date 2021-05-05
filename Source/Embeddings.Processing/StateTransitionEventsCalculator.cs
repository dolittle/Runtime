// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="ICalculateStateTransitionEvents" />.
    /// </summary>
    public class StateTransitionEventsCalculator : ICalculateStateTransitionEvents
    {
        readonly EmbeddingId _identifier;
        readonly IEmbedding _embedding;
        readonly IProjectManyEvents _projector;
        readonly ICompareStates _stateComparer;
        readonly IDetectEmbeddingLoops _loopDetector;

        /// <summary>
        /// Initializes an instance of the <see cref="StateTransitionEventsCalculator" /> class.
        /// </summary>
        /// <param name="identifier">The <see cref="EmbeddingId" />.</param>
        /// <param name="embedding">The <see cref="IEmbedding" />.</param>
        /// <param name="projector">The <see cref="IProjectManyEvents"/>.</param>
        /// <param name="stateComparer">The <see cref="ICompareStates"/>.</param>
        /// <param name="loopDetector">The <see cref="IDetectEmbeddingLoops"/>.</param>
        public StateTransitionEventsCalculator(
            EmbeddingId identifier,
            IEmbedding embedding,
            IProjectManyEvents projector,
            ICompareStates stateComparer,
            IDetectEmbeddingLoops loopDetector
        )
        {
            _identifier = identifier;
            _embedding = embedding;
            _projector = projector;
            _stateComparer = stateComparer;
            _loopDetector = loopDetector;
        }

        /// <inheritdoc/>
        public async Task<Try<UncommittedAggregateEvents>> TryConverge(EmbeddingCurrentState current, ProjectionState desired, CancellationToken cancellationToken)
        {
            var allTransitionEvents = new List<UncommittedEvents>();
            while (true)
            {
                var statesAreEqualResult = await _stateComparer.TryCheckEquality(current.State, desired).ConfigureAwait(false);
                if (!statesAreEqualResult.Success)
                {
                    return statesAreEqualResult.Exception;
                }

                if (statesAreEqualResult.Result)
                {
                    var events = from uncommittedEvents in allTransitionEvents
                                 from @event in uncommittedEvents
                                 select @event;

                    return CreateUncommittedAggregateEvents(new UncommittedEvents(events.ToArray()), current);

                }

                var transitionEvents = await _embedding.TryCompare(current, desired, cancellationToken).ConfigureAwait(false);
                if (!transitionEvents.Success)
                {
                    return transitionEvents.Exception;
                }

                allTransitionEvents.Add(transitionEvents.Result);

                var loopDetected = await _loopDetector.TryCheckEventLoops(allTransitionEvents).ConfigureAwait(false);
                if (!loopDetected.Success)
                {
                    return loopDetected.Exception;
                }

                if (loopDetected.Result)
                {
                    return new EmbeddingLoopDetected(_identifier);
                }

                var intermediateState = await _projector.TryProject(current, transitionEvents.Result, cancellationToken).ConfigureAwait(false);
                if (!intermediateState.Success)
                {
                    return intermediateState.IsPartialResult
                        ? new CouldNotProjectAllEvents(_identifier)
                        : new FailedProjectingEvents(_identifier);
                }

                current = intermediateState.Result;
            }
        }

        /// <inheritdoc/>
        public Task<Try<UncommittedAggregateEvents>> TryDelete(EmbeddingCurrentState current, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        UncommittedAggregateEvents CreateUncommittedAggregateEvents(UncommittedEvents events, EmbeddingCurrentState currentState)
            => new(_identifier.Value, new Artifact(_identifier.Value, ArtifactGeneration.First), currentState.Version, events);
    }
}
