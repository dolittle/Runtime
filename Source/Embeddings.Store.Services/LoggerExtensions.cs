// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Store.Services
{
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, string, Exception> _gettingOneEmbedding = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Debug,
                new EventId(359275420, nameof(GettingOneEmbedding)),
                "Getting state for embedding {Embedding} with key {Key}");



        internal static void GettingOneEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key)
            => _gettingOneEmbedding(logger, embedding, key, null);

        static readonly Action<ILogger, Guid, Exception> _gettingAllEmbeddings = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(276362862, nameof(GettingAllEmbeddings)),
                "Getting all states for Embedding {Embedding}");

        internal static void GettingAllEmbeddings(this ILogger logger, EmbeddingId embedding)
            => _gettingAllEmbeddings(logger, embedding, null);

        static readonly Action<ILogger, Guid, Exception> _gettingAllKeys = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(247104242, nameof(GettingAllKeys)),
                "Getting keys for Embedding {Embedding}");

        internal static void GettingAllKeys(this ILogger logger, EmbeddingId embedding)
            => _gettingAllKeys(logger, embedding, null);
    }
}