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
        readonly IConvertProjectionKeysToEventSourceIds _keyToEventSourceConverter;

        /// <summary>
        /// Initializes an instance of the <see cref="StateTransitionEventsCalculator" /> class.
        /// </summary>
        /// <param name="identifier">The <see cref="EmbeddingId" />.</param>
        /// <param name="embedding">The <see cref="IEmbedding" />.</param>
        /// <param name="projector">The <see cref="IProjectManyEvents"/>.</param>
        /// <param name="stateComparer">The <see cref="ICompareStates"/>.</param>
        /// <param name="loopDetector">The <see cref="IDetectEmbeddingLoops"/>.</param>
        /// <param name="keyToEventSourceConverter">The <see cref="IConvertProjectionKeysToEventSourceIds"/>.</param>
        public StateTransitionEventsCalculator(
            EmbeddingId identifier,
            IEmbedding embedding,
            IProjectManyEvents projector,
            ICompareStates stateComparer,
            IDetectEmbeddingLoops loopDetector,
            IConvertProjectionKeysToEventSourceIds keyToEventSourceConverter)
        {
            _identifier = identifier;
            _embedding = embedding;
            _projector = projector;
            _stateComparer = stateComparer;
            _loopDetector = loopDetector;
            _keyToEventSourceConverter = keyToEventSourceConverter;
        }

        /// <inheritdoc/>
        public Task<Try<UncommittedAggregateEvents>> TryConverge(EmbeddingCurrentState current, ProjectionState desired, CancellationToken cancellationToken)
            => DoWork(
                current,
                newCurrent => _stateComparer.TryCheckEquality(newCurrent.State, desired),
                (newCurrent, token) => _embedding.TryCompare(newCurrent, desired, token),
                cancellationToken);

        /// <inheritdoc/>
        public Task<Try<UncommittedAggregateEvents>> TryDelete(EmbeddingCurrentState current, CancellationToken cancellationToken)
            => DoWork(
                current,
                newCurrent => Try<bool>.Do(() => newCurrent.Type is EmbeddingCurrentStateType.Deleted),
                (newCurrent, token) => _embedding.TryDelete(newCurrent, token),
                cancellationToken);

        async Task<Try<UncommittedAggregateEvents>> DoWork(
            EmbeddingCurrentState current,
            Func<EmbeddingCurrentState, Try<bool>> isDesiredState,
            Func<EmbeddingCurrentState, CancellationToken, Task<Try<UncommittedEvents>>> getTransitionEvents,
            CancellationToken cancellationToken)
        {
            try
            {
                var allTransitionEvents = new List<UncommittedEvents>();
                var previousStates = new List<ProjectionState>
                {
                    current.State
                };

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return new CalculateStateTransitionEventsCancelled(_identifier);
                    }

                    var isDesired = isDesiredState(current);
                    if (!isDesired.Success)
                    {
                        return isDesired.Exception;
                    }
                    if (isDesired.Result)
                    {
                        var events = from uncommittedEvents in allTransitionEvents
                                     from @event in uncommittedEvents
                                     select @event;

                        return CreateUncommittedAggregateEvents(new UncommittedEvents(events.ToArray()), current);
                    }

                    var transitionEvents = await getTransitionEvents(current, cancellationToken).ConfigureAwait(false);
                    if (!transitionEvents.Success)
                    {
                        return transitionEvents.Exception;
                    }

                    allTransitionEvents.Add(transitionEvents.Result);

                    var intermediateState = await _projector.TryProject(current, transitionEvents.Result, cancellationToken).ConfigureAwait(false);
                    if (!intermediateState.Success)
                    {
                        return intermediateState.IsPartialResult
                            ? new CouldNotProjectAllEvents(_identifier, intermediateState.Exception)
                            : new FailedProjectingEvents(_identifier, intermediateState.Exception);
                    }

                    var loopDetected = _loopDetector.TryCheckForProjectionStateLoop(intermediateState.Result.State, previousStates);
                    if (!loopDetected.Success)
                    {
                        return loopDetected.Exception;
                    }

                    if (loopDetected.Result)
                    {
                        return new EmbeddingLoopDetected(_identifier);
                    }

                    previousStates.Add(intermediateState.Result.State);
                    current = intermediateState.Result;
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        UncommittedAggregateEvents CreateUncommittedAggregateEvents(UncommittedEvents events, EmbeddingCurrentState currentState)
            => new(
                _keyToEventSourceConverter.GetEventSourceIdFor(currentState.Key),
                new Artifact(_identifier.Value, ArtifactGeneration.First),
                currentState.Version,
                events);
    }
}
