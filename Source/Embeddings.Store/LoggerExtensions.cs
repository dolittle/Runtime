// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Store;

static class LoggerExtensions
{
    static readonly Action<ILogger, Guid, string, Exception> _gettingOneEmbedding = LoggerMessage
        .Define<Guid, string>(
            LogLevel.Debug,
            new EventId(2006779877, nameof(GettingOneEmbedding)),
            "Getting state for embedding {Embedding} with key {Key}");

    internal static void GettingOneEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key)
        => _gettingOneEmbedding(logger, embedding, key, null);

    static readonly Action<ILogger, Guid, string, Exception> _errorGettingOneEmbedding = LoggerMessage
        .Define<Guid, string>(
            LogLevel.Warning,
            new EventId(235391867, nameof(ErrorGettingOneEmbedding)),
            "Error getting state for embedding {Embedding} with key {Key}");

    internal static void ErrorGettingOneEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key, Exception exception)
        => _errorGettingOneEmbedding(logger, embedding, key, exception);

    static readonly Action<ILogger, Guid, Exception> _gettingAllEmbeddings = LoggerMessage
        .Define<Guid>(
            LogLevel.Debug,
            new EventId(340579607, nameof(GettingAllEmbeddings)),
            "Getting all states for embedding {Embedding}");

    internal static void GettingAllEmbeddings(this ILogger logger, EmbeddingId embedding)
        => _gettingAllEmbeddings(logger, embedding, null);

    static readonly Action<ILogger, Guid, Exception> _errorGettingAllEmbeddings = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(1901764417, nameof(ErrorGettingAllEmbeddings)),
            "Error getting all states for embedding {Embedding}");

    internal static void ErrorGettingAllEmbeddings(this ILogger logger, EmbeddingId embedding, Exception exception)
        => _errorGettingAllEmbeddings(logger, embedding, exception);

    static readonly Action<ILogger, Guid, Exception> _gettingEmbeddingKeys = LoggerMessage
        .Define<Guid>(
            LogLevel.Debug,
            new EventId(403575857, nameof(GettingEmbeddingKeys)),
            "Getting keys for embedding {Embedding}");

    internal static void GettingEmbeddingKeys(this ILogger logger, EmbeddingId embedding)
        => _gettingEmbeddingKeys(logger, embedding, null);

    static readonly Action<ILogger, Guid, Exception> _errorGettingEmbeddingKeys = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(298297742, nameof(ErrorGettingEmbeddingKeys)),
            "Error getting keys for embedding {Embedding}");

    internal static void ErrorGettingEmbeddingKeys(this ILogger logger, EmbeddingId embedding, Exception exception)
        => _errorGettingEmbeddingKeys(logger, embedding, exception);

    static readonly Action<ILogger, Guid, string, ulong, Exception> _removingEmbedding = LoggerMessage
        .Define<Guid, string, ulong>(
            LogLevel.Debug,
            new EventId(293285248, nameof(RemovingEmbedding)),
            "Removing embedding with id {Embedding}, key {Key} and version {Version}");

    internal static void RemovingEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version)
        => _removingEmbedding(logger, embedding, key, version, null);

    static readonly Action<ILogger, Guid, string, ulong, Exception> _errorRemovingEmbedding = LoggerMessage
        .Define<Guid, string, ulong>(
            LogLevel.Warning,
            new EventId(87120798, nameof(ErrorRemovingEmbedding)),
            "Error removing embedding with id {Embedding}, key {Key} and version {Version}");

    internal static void ErrorRemovingEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, Exception exception)
        => _errorRemovingEmbedding(logger, embedding, key, version, exception);

    static readonly Action<ILogger, Guid, string, ulong, string, Exception> _replacingEmbedding = LoggerMessage
        .Define<Guid, string, ulong, string>(
            LogLevel.Debug,
            new EventId(646882857, nameof(ReplacingEmbedding)),
            "Replacing embedding with id {Embedding}, key {Key} and version {Version} with state {State}");

    internal static void ReplacingEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, ProjectionState state)
        => _replacingEmbedding(logger, embedding, key, version, state, null);

    static readonly Action<ILogger, Guid, string, ulong, string, Exception> _errorReplacingEmbedding = LoggerMessage
        .Define<Guid, string, ulong, string>(
            LogLevel.Warning,
            new EventId(221665652, nameof(ErrorReplacingEmbedding)),
            "Error replacing embedding with id {Embedding}, key {Key} and version {Version} with state {State}");

    internal static void ErrorReplacingEmbedding(this ILogger logger, EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, ProjectionState state, Exception exception)
        => _errorReplacingEmbedding(logger, embedding, key, version, state, exception);
}