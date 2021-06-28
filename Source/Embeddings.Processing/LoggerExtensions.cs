// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing
{
    internal static class LoggerExtensions
    {
        #region EmbeddingsService

        static readonly Action<ILogger, Guid, Exception> _comparingEmbeddingDefinition = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(208111084, nameof(ComparingEmbeddingDefinition)),
                "Comparing embedding definition for embedding {Embedding}");

        internal static void ComparingEmbeddingDefinition(this ILogger logger, EmbeddingDefinition arguments)
            => _comparingEmbeddingDefinition(logger, arguments.Embedding, null);

        static readonly Action<ILogger, Guid, string, Exception> _invalidEmbeddingDefinition = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Warning,
                new EventId(1752597594, nameof(InvalidEmbeddingDefinition)),
                "Embedding definition for embedding {Embedding} failed validation. {Reason}");

        internal static void InvalidEmbeddingDefinition(this ILogger logger, EmbeddingDefinition defintion, IEnumerable<(TenantId, EmbeddingDefinitionComparisonResult)> failedComparisons)
            => _invalidEmbeddingDefinition(
                logger,
                defintion.Embedding,
                failedComparisons.First().Item2.FailureReason,
                null);

        static readonly Action<ILogger, Guid, Exception> _persistingDefinition = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(220634741, nameof(PersistingDefinition)),
                "Persisting definition for Embedding {Embedding}");

        internal static void PersistingDefinition(this ILogger logger, EmbeddingId embedding)
            => _persistingDefinition(
                logger,
                embedding,
                null);
        #endregion

        #region StateTransitionEventsCalculator
        static readonly Action<ILogger, Guid, string, ulong, Exception> _calculatingStateTransitionEvents = LoggerMessage
            .Define<Guid, string, ulong>(
                LogLevel.Debug,
                new EventId(1563454567, nameof(CalculatingStateTransitionEvents)),
                "Calculating state transition events for embedding {Embedding} with key {Key} at aggregate root version {Version}");

        internal static void CalculatingStateTransitionEvents(this ILogger logger, EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version)
            => _calculatingStateTransitionEvents(logger, embedding, key, version, null);

        static readonly Action<ILogger, Guid, string, ulong, Exception> _calculatingStateTransitionEventsCancelled = LoggerMessage
            .Define<Guid, string, ulong>(
                LogLevel.Warning,
                new EventId(412978046, nameof(CalculatingStateTransitionEventsCancelled)),
                "Calculating state transition events was cancellled for embedding {Embedding} with key {Key} at aggregate root version {Version}");

        internal static void CalculatingStateTransitionEventsCancelled(this ILogger logger, EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version)
            => _calculatingStateTransitionEventsCancelled(logger, embedding, key, version, null);

        static readonly Action<ILogger, Guid, string, ulong, Exception> _calculatingStateTransitionEventsFailedCheckingIfDesiredState = LoggerMessage
            .Define<Guid, string, ulong>(
                LogLevel.Warning,
                new EventId(327860911, nameof(CalculatingStateTransitionEventsFailedCheckingIfDesiredState)),
                "Calculating state transition events failed while checking if the current state is the desired state for embedding {Embedding} with key {Key} at aggregate root version {Version}");

        internal static void CalculatingStateTransitionEventsFailedCheckingIfDesiredState(
            this ILogger logger,
            EmbeddingId embedding,
            ProjectionKey key,
            AggregateRootVersion version,
            Exception exception)
            => _calculatingStateTransitionEventsFailedCheckingIfDesiredState(logger, embedding, key, version, exception);

        static readonly Action<ILogger, Guid, string, ulong, Exception> _calculatingStateTransitionEventsFailedGettingNextTransitionEvents = LoggerMessage
            .Define<Guid, string, ulong>(
                LogLevel.Warning,
                new EventId(226703386, nameof(CalculatingStateTransitionEventsFailedGettingNextTransitionEvents)),
                "Calculating state transition events failed getting the next transition events for embedding {Embedding} with key {Key} at aggregate root version {Version}");

        internal static void CalculatingStateTransitionEventsFailedGettingNextTransitionEvents(
            this ILogger logger,
            EmbeddingId embedding,
            ProjectionKey key,
            AggregateRootVersion version,
            Exception exception)
            => _calculatingStateTransitionEventsFailedGettingNextTransitionEvents(logger, embedding, key, version, exception);

        static readonly Action<ILogger, Guid, string, ulong, Exception> _calculatingStateTransitionEventsFailedProjectingNewState = LoggerMessage
            .Define<Guid, string, ulong>(
                LogLevel.Warning,
                new EventId(81456606, nameof(CalculatingStateTransitionEventsFailedProjectingNewState)),
                "Calculating state transition events failed projecting the new state from the transition events for embedding {Embedding} with key {Key} at aggregate root version {Version}");

        internal static void CalculatingStateTransitionEventsFailedProjectingNewState(
            this ILogger logger,
            EmbeddingId embedding,
            ProjectionKey key,
            AggregateRootVersion version,
            Exception exception)
            => _calculatingStateTransitionEventsFailedProjectingNewState(logger, embedding, key, version, exception);

        #endregion
        #region EmbeddingStateUpdater
        static readonly Action<ILogger, Guid, Exception> _updatingAllEmbeddingStates = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(157937906, nameof(UpdatingAllEmbeddingStates)),
                "Updating all embeddings states for embedding {Embedding}");

        internal static void UpdatingAllEmbeddingStates(this ILogger logger, EmbeddingId embedding)
            => _updatingAllEmbeddingStates(logger, embedding, null);

        static readonly Action<ILogger, Guid, string, Exception> _updatingEmbeddingStateFor = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Trace,
                new EventId(401955945, nameof(UpdatingEmbeddingStateFor)),
                "Updating embedding state for embedding {Embedding} and key {Key}");

        internal static void UpdatingEmbeddingStateFor(this ILogger logger, EmbeddingId embedding, ProjectionKey key)
            => _updatingEmbeddingStateFor(logger, embedding, key, null);

        static readonly Action<ILogger, Guid, string, Exception> _failedUpdatingEmbeddingStateFor = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Warning,
                new EventId(30417, nameof(FailedUpdatingEmbeddingStateFor)),
                "Failed updating embedding state for embedding {Embedding} and key {Key}");

        internal static void FailedUpdatingEmbeddingStateFor(this ILogger logger, EmbeddingId embedding, ProjectionKey key, Exception exception)
            => _failedUpdatingEmbeddingStateFor(logger, embedding, key, exception);

        #endregion
        #region EmbeddingProcessor
        static readonly Action<ILogger, Guid, Exception> _failedEnsuringAllStatesAreFresh = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(291050328, nameof(FailedEnsuringAllStatesAreFresh)),
                "Failed to ensure all embedding states are fresh for embedding {Embedding}");

        internal static void FailedEnsuringAllStatesAreFresh(this ILogger logger, EmbeddingId embedding, Exception exception)
            => _failedEnsuringAllStatesAreFresh(logger, embedding, exception);

        static readonly Action<ILogger, Guid, string, Exception> _eventProcessorWorkWasCancelled = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Warning,
                new EventId(43544717, nameof(EventProcessorWorkWasCancelled)),
                "Cancelled embedding processor with id {Embedding} while deleting or updating embedding state with key {Key}");

        internal static void EventProcessorWorkWasCancelled(this ILogger logger, EmbeddingId embedding, ProjectionKey key)
            => _eventProcessorWorkWasCancelled(logger, embedding, key, null);

        static readonly Action<ILogger, Guid, string, Exception> _eventProcessorWorkFailed = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Warning,
                new EventId(78426396, nameof(EventProcessorWorkFailed)),
                "An error ocurred while updating or deleting embedding {Embedding} with key {Key}");

        internal static void EventProcessorWorkFailed(this ILogger logger, EmbeddingId embedding, ProjectionKey key, Exception exception)
            => _eventProcessorWorkFailed(logger, embedding, key, exception);

        static readonly Action<ILogger, Guid, int, string, Exception> _committingTransitionEvents = LoggerMessage
            .Define<Guid, int, string>(
                LogLevel.Debug,
                new EventId(319, nameof(CommittingTransitionEvents)),
                "Embedding processor with id {Embedding} is committing {NumberOfEvents} transition events for state with key {Key}");

        internal static void CommittingTransitionEvents(this ILogger logger, EmbeddingId embedding, ProjectionKey key, UncommittedAggregateEvents events)
            => _committingTransitionEvents(logger, embedding, events.Count, key, null);

        #endregion
        #region EmbeddingProcessors
        static readonly Action<ILogger, Guid, Exception> _startingEmbeddiingProcessorForAllTenants = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(371525267, nameof(StartingEmbeddiingProcessorForAllTenants)),
                "Trying to start embedding processor with id {Embedding} for all tenants");

        internal static void StartingEmbeddiingProcessorForAllTenants(this ILogger logger, EmbeddingId embedding)
            => _startingEmbeddiingProcessorForAllTenants(logger, embedding, null);

        static readonly Action<ILogger, Guid, Exception> _failedRegisteringEmbeddingProcessor = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(380586739, nameof(FailedRegisteringEmbeddingProcessor)),
                "Failed to registering embedding processor with id {Embedding}");

        internal static void FailedRegisteringEmbeddingProcessor(this ILogger logger, EmbeddingId embedding, Exception exception)
            => _failedRegisteringEmbeddingProcessor(logger, embedding, exception);

        static readonly Action<ILogger, Guid, Exception> _embeddingProcessorsStarted = LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(112400393, nameof(EmbeddingProcessorsStarted)),
                "All embedding processors with id {Embedding} has started");

        internal static void EmbeddingProcessorsStarted(this ILogger logger, EmbeddingId embedding)
            => _embeddingProcessorsStarted(logger, embedding, null);

        static readonly Action<ILogger, Guid, Exception> _stoppingAllEmbeddingProcessors = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(289216221, nameof(StoppingAllEmbeddingProcessors)),
                "Stopping all embedding processors with id {Embedding}");

        internal static void StoppingAllEmbeddingProcessors(this ILogger logger, EmbeddingId embedding)
            => _stoppingAllEmbeddingProcessors(logger, embedding, null);

        static readonly Action<ILogger, Guid, Exception> _allEmbeddingProcessorsSuccessfullyStopped = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(396379390, nameof(AllEmbeddingProcessorsSuccessfullyStopped)),
                "All embedding processors with id {Embedding} has successfully been stopped");

        internal static void AllEmbeddingProcessorsSuccessfullyStopped(this ILogger logger, EmbeddingId embedding)
            => _allEmbeddingProcessorsSuccessfullyStopped(logger, embedding, null);

        static readonly Action<ILogger, Guid, Exception> _anErrorOccurredWhileStartingOrStoppingEmbedding = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(3037276, nameof(AnErrorOccurredWhileStartingOrStoppingEmbedding)),
                "An error ocurred while starting or stopping embedding processors with id {Embedding}");

        internal static void AnErrorOccurredWhileStartingOrStoppingEmbedding(this ILogger logger, EmbeddingId embedding, Exception exception)
            => _anErrorOccurredWhileStartingOrStoppingEmbedding(logger, embedding, exception);

        #endregion
        #region ProjectManyEvents
        static readonly Action<ILogger, int, Guid, string, Exception> _projectingEventsOnEmbedding = LoggerMessage
            .Define<int, Guid, string>(
                LogLevel.Debug,
                new EventId(11584329, nameof(ProjectingEventsOnEmbedding)),
                "Projecting {NumEvents} events on embedding {Embedding} and key {Key}");

        internal static void ProjectingEventsOnEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key, UncommittedEvents events)
            => _projectingEventsOnEmbedding(logger, events.Count, embedding, key, null);

        #endregion
        #region Embedding
        static readonly Action<ILogger, Guid, string, string, Exception> _projectEventThroughDispatcher = LoggerMessage
            .Define<Guid, string, string>(
                LogLevel.Debug,
                new EventId(33587349, nameof(ProjectEventThroughDispatcher)),
                "Projecting an event on embedding {Embedding} and key {Key} with current state of type {Type}");

        internal static void ProjectEventThroughDispatcher(this ILogger logger, EmbeddingId embedding, ProjectionCurrentState state)
            => _projectEventThroughDispatcher(logger, embedding, state.Key, Enum.GetName(state.Type), null);

        static readonly Action<ILogger, Guid, string, string, string, string, Exception> _compareStatesForEmbedding = LoggerMessage
            .Define<Guid, string, string, string, string>(
                LogLevel.Debug,
                new EventId(268001296, nameof(CompareStatesForEmbedding)),
                "Comparing states for embedding {Embedding} and key {Key} with current state of type {Type}. Current state is {CurrentState} and desired state is {DesiredState}");

        internal static void CompareStatesForEmbedding(this ILogger logger, EmbeddingId embedding, EmbeddingCurrentState currentState, ProjectionState desiredState)
            => _compareStatesForEmbedding(logger, embedding, currentState.Key, Enum.GetName(currentState.Type), currentState.State, desiredState, null);

        static readonly Action<ILogger, Guid, string, string, Exception> _deletingStateForEmbedding = LoggerMessage
            .Define<Guid, string, string>(
                LogLevel.Debug,
                new EventId(22170214, nameof(DeletingStateForEmbedding)),
                "Deleting state for embedding {Embedding} and key {Key} with current state of type {Type}");

        internal static void DeletingStateForEmbedding(this ILogger logger, EmbeddingId embedding, EmbeddingCurrentState currentState)
            => _deletingStateForEmbedding(logger, embedding, currentState.Key, Enum.GetName(currentState.Type), null);
        #endregion

    }
}
