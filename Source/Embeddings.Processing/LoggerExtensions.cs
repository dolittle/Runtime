// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing
{
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, Exception> _gettingOneEmbedding = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(208111084, nameof(ComparingEmbeddingDefinition)),
                "Comparing embedding definition for embedding {Embedding}");

        internal static void ComparingEmbeddingDefinition(this ILogger logger, EmbeddingDefinition arguments)
            => _gettingOneEmbedding(logger, arguments.Embedding, null);

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
    }
}
