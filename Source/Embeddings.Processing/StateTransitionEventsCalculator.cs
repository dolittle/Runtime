// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="ICalculateStateTransitionEvents" />.
/// </summary>
[DisableAutoRegistration]
public class StateTransitionEventsCalculator : ICalculateStateTransitionEvents
{
    readonly EmbeddingId _embeddingId;
    readonly IEmbedding _embedding;
    readonly IProjectManyEvents _projector;
    readonly ICompareStates _stateComparer;
    readonly IDetectEmbeddingLoops _loopDetector;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes an instance of the <see cref="StateTransitionEventsCalculator" /> class.
    /// </summary>
    /// <param name="identifier">The <see cref="EmbeddingId" />.</param>
    /// <param name="embedding">The <see cref="IEmbedding" />.</param>
    /// <param name="projector">The <see cref="IProjectManyEvents"/>.</param>
    /// <param name="stateComparer">The <see cref="ICompareStates"/>.</param>
    /// <param name="loopDetector">The <see cref="IDetectEmbeddingLoops"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public StateTransitionEventsCalculator(
        EmbeddingId identifier,
        IEmbedding embedding,
        IProjectManyEvents projector,
        ICompareStates stateComparer,
        IDetectEmbeddingLoops loopDetector,
        ILogger logger)
    {
        _embeddingId = identifier;
        _embedding = embedding;
        _projector = projector;
        _stateComparer = stateComparer;
        _loopDetector = loopDetector;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<Try<UncommittedAggregateEvents>> TryConverge(EmbeddingCurrentState current, ProjectionState desired, ExecutionContext executionContext, CancellationToken cancellationToken)
        => DoWork(
            current,
            newCurrent => _stateComparer.TryCheckEquality(newCurrent.State, desired),
            (newCurrent, token) => _embedding.TryCompare(newCurrent, desired, executionContext, token),
            executionContext,
            cancellationToken);

    /// <inheritdoc/>
    public Task<Try<UncommittedAggregateEvents>> TryDelete(EmbeddingCurrentState current, ExecutionContext executionContext, CancellationToken cancellationToken)
        => DoWork(
            current,
            newCurrent => Try<bool>.Do(() => newCurrent.Type is EmbeddingCurrentStateType.Deleted),
            (newCurrent, token) => _embedding.TryDelete(newCurrent, executionContext, token),
            executionContext,
            cancellationToken);

    async Task<Try<UncommittedAggregateEvents>> DoWork(
        EmbeddingCurrentState current,
        Func<EmbeddingCurrentState, Try<bool>> isDesiredState,
        Func<EmbeddingCurrentState, CancellationToken, Task<Try<UncommittedEvents>>> getTransitionEvents,
        ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.CalculatingStateTransitionEvents(_embeddingId, current.Key, current.Version);
            var allTransitionEvents = new List<UncommittedEvents>();
            var previousStates = new List<ProjectionState>
            {
                current.State
            };

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.CalculatingStateTransitionEventsCancelled(_embeddingId, current.Key, current.Version);
                    return new CalculateStateTransitionEventsCancelled(_embeddingId);
                }
                var getEventsToCommit = TryCheckIfDesiredState(current, allTransitionEvents, isDesiredState, out var eventsToCommit);
                if (!getEventsToCommit.Success || getEventsToCommit.Result)
                {
                    if (getEventsToCommit.Exception != default)
                    {
                        _logger.CalculatingStateTransitionEventsFailedCheckingIfDesiredState(
                            _embeddingId,
                            current.Key,
                            current.Version,
                            getEventsToCommit.Exception);
                        return getEventsToCommit.Exception;
                    }
                    return eventsToCommit;
                }
                var addNewTransitionEvents = await TryGetAndAddNewTransitionEventsInto(
                    allTransitionEvents,
                    current,
                    getTransitionEvents,
                    cancellationToken).ConfigureAwait(false);
                if (!addNewTransitionEvents.Success)
                {
                    _logger.CalculatingStateTransitionEventsFailedGettingNextTransitionEvents(
                        _embeddingId,
                        current.Key,
                        current.Version,
                        addNewTransitionEvents.Exception);
                    return addNewTransitionEvents.Exception;
                }

                var projectIntermediateState = await TryProjectNewState(current, addNewTransitionEvents, previousStates, executionContext, cancellationToken).ConfigureAwait(false);
                if (!projectIntermediateState.Success)
                {
                    _logger.CalculatingStateTransitionEventsFailedProjectingNewState(
                        _embeddingId,
                        current.Key,
                        current.Version,
                        addNewTransitionEvents.Exception);
                    return projectIntermediateState.Exception;
                }

                previousStates.Add(projectIntermediateState.Result.State);
                current = projectIntermediateState;
            }
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    Try<bool> TryCheckIfDesiredState(
        EmbeddingCurrentState current,
        IEnumerable<UncommittedEvents> allTransitionEvents,
        Func<EmbeddingCurrentState, Try<bool>> isDesiredState,
        out UncommittedAggregateEvents eventsToCommit)
    {
        eventsToCommit = null;
        var isDesired = isDesiredState(current);
        if (!isDesired.Success)
        {
            return isDesired.Exception;
        }
        if (!isDesired.Result)
        {
            return false;
        }
        var flattenedTransitionEvents = from uncommittedEvents in allTransitionEvents
                                        from @event in uncommittedEvents
                                        select @event;

        eventsToCommit = CreateUncommittedAggregateEvents(new UncommittedEvents(flattenedTransitionEvents.ToArray()), current);
        return true;
    }

    async Task<Try<UncommittedEvents>> TryGetAndAddNewTransitionEventsInto(
        IList<UncommittedEvents> allTransitionEvents,
        EmbeddingCurrentState current,
        Func<EmbeddingCurrentState, CancellationToken, Task<Try<UncommittedEvents>>> getTransitionEvents,
        CancellationToken cancellationToken)
    {
        var newTransitionEvents = await getTransitionEvents(current, cancellationToken).ConfigureAwait(false);
        if (!newTransitionEvents.Success)
        {
            return newTransitionEvents.Exception;
        }
        allTransitionEvents.Add(newTransitionEvents.Result);
        return newTransitionEvents;
    }

    async Task<Try<EmbeddingCurrentState>> TryProjectNewState(
        EmbeddingCurrentState current,
        Try<UncommittedEvents> newTransitionEvents,
        IEnumerable<ProjectionState> previousStates,
        ExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        var intermediateState = await _projector.TryProject(current, newTransitionEvents.Result, executionContext, cancellationToken).ConfigureAwait(false);
        if (!intermediateState.Success)
        {
            return intermediateState.IsPartialResult
                ? new CouldNotProjectAllEvents(_embeddingId, intermediateState.Exception)
                : new FailedProjectingEvents(_embeddingId, intermediateState.Exception);
        }

        var loopDetected = _loopDetector.TryCheckForProjectionStateLoop(intermediateState.Result.State, previousStates);

        return loopDetected switch
        {
            { Success: false } => loopDetected.Exception,
            { Result: true } => new EmbeddingLoopDetected(_embeddingId),
            _ => intermediateState
        };
    }

    UncommittedAggregateEvents CreateUncommittedAggregateEvents(UncommittedEvents events, EmbeddingCurrentState currentState)
        => new(
            currentState.Key.Value,
            new Artifact(_embeddingId.Value, ArtifactGeneration.First),
            currentState.Version.Value - (ulong)events.Count,
            events);
}
