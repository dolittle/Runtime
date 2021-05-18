// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Store
{
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, string, Exception> _gettingOneEmbedding = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Debug,
                new EventId(2006779877, nameof(GettingOneEmbedding)),
                "Getting state for embedding {Embedding} with key {Key}");

        static readonly Action<ILogger, Guid, Exception> _gettingAllEmbeddings = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(340579607, nameof(GettingAllEmbeddings)),
                "Getting all states for embedding {Embedding}");

        static readonly Action<ILogger, Guid, Exception> _gettingEmbeddingKeys = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(403575857, nameof(GettingEmbeddingKeys)),
                "Getting keys for embedding {Embedding}");

        static readonly Action<ILogger, Guid, string, ulong, Exception> _removingEmbedding = LoggerMessage
            .Define<Guid, string, ulong>(
                LogLevel.Debug,
                new EventId(293285248, nameof(RemovingEmbedding)),
                "Removing embedding with id {Embedding}, key {Key} and version {Version}");

        static readonly Action<ILogger, Guid, string, ulong, string, Exception> _replacingEmbedding = LoggerMessage
            .Define<Guid, string, ulong, string>(
                LogLevel.Debug,
                new EventId(646882857, nameof(ReplacingEmbedding)),
                "Replacing embedding with id {Embedding}, key {Key} and version {Version} with state {State}");

        internal static void GettingOneEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key)
            => _gettingOneEmbedding(logger, embedding, key, null);

        internal static void GettingAllEmbeddings(this ILogger logger, EmbeddingId embedding)
            => _gettingAllEmbeddings(logger, embedding, null);

        internal static void GettingEmbeddingKeys(this ILogger logger, EmbeddingId embedding)
            => _gettingEmbeddingKeys(logger, embedding, null);

        internal static void RemovingEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version)
            => _removingEmbedding(logger, embedding, key, version, null);

        internal static void ReplacingEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, ProjectionState state)
            => _replacingEmbedding(logger, embedding, key, version, state, null);
    }
}
