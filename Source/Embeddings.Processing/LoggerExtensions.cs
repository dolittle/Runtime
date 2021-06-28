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
    }
}
