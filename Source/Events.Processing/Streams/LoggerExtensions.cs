// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    static class LoggerExtensions
    {
        static readonly Action<ILogger, StreamProcessorId, Exception> _initializingStreamProcessor = LoggerMessage
            .Define<StreamProcessorId>(
                LogLevel.Debug,
                new EventId(1617407982, nameof(InitializingStreamProcessor)),
                "Initializing stream processor with id: {StreamProcessorId}");

        static readonly Action<ILogger, StreamProcessorId, Exception> _startingStreamProcessor = LoggerMessage
            .Define<StreamProcessorId>(
                LogLevel.Debug,
                new EventId(698891248, nameof(StartingStreamProcessor)),
                "Starting stream processor with id: {StreamProcessorId}");

        static readonly Action<ILogger, StreamProcessorId, Exception> _scopedStreamProcessorFailed = LoggerMessage
            .Define<StreamProcessorId>(
                LogLevel.Warning,
                new EventId(146285673, nameof(ScopedStreamProcessorFailed)),
                "A failure ocurred in a scoped stream processor with id: {StreamProcessorId}");
        static readonly Action<ILogger, IStreamProcessorId, StreamPosition, Exception> _scopedStreamProcessorSetToPosition = LoggerMessage
            .Define<IStreamProcessorId, StreamPosition>(
                LogLevel.Information,
                new EventId(16542673, nameof(ScopedStreamProcessorSetToPosition)),
                "Stream Processor: {StreamProcessorId} position has been set to {Position}");

        static readonly Action<ILogger, StreamProcessorId, Exception> _streamProcessorAlreadyRegistered = LoggerMessage
            .Define<StreamProcessorId>(
                LogLevel.Warning,
                new EventId(269101514, nameof(ScopedStreamProcessorFailed)),
                "There is already registed a Stream processor with id: {StreamProcessorId}");

        static readonly Action<ILogger, StreamProcessorId, Exception> _streamProcessorSuccessfullyRegistered = LoggerMessage
            .Define<StreamProcessorId>(
                LogLevel.Trace,
                new EventId(1784995803, nameof(ScopedStreamProcessorFailed)),
                "Successfully registed stream processor with id: {StreamProcessorId}");

        static readonly Action<ILogger, StreamProcessorId, Exception> _streamProcessorUnregistered = LoggerMessage
            .Define<StreamProcessorId>(
                LogLevel.Trace,
                new EventId(2095134562, nameof(ScopedStreamProcessorFailed)),
                "Unregistering stream processor with id: {StreamProcessorId}");

        internal static void InitializingStreamProcessor(this ILogger logger, StreamProcessorId streamProcessorId)
            => _initializingStreamProcessor(logger, streamProcessorId, null);

        internal static void StartingStreamProcessor(this ILogger logger, StreamProcessorId streamProcessorId)
            => _startingStreamProcessor(logger, streamProcessorId, null);

        internal static void ScopedStreamProcessorFailed(this ILogger logger, Exception ex, StreamProcessorId streamProcessorId)
            => _scopedStreamProcessorFailed(logger, streamProcessorId, ex);
        internal static void ScopedStreamProcessorSetToPosition(this ILogger logger, IStreamProcessorId streamProcessorId, StreamPosition position)
            => _scopedStreamProcessorSetToPosition(logger, streamProcessorId, position, null);

        internal static void StreamProcessorAlreadyRegistered(this ILogger logger, StreamProcessorId streamProcessorId)
            => _streamProcessorAlreadyRegistered(logger, streamProcessorId, null);

        internal static void StreamProcessorSuccessfullyRegistered(this ILogger logger, StreamProcessorId streamProcessorId)
            => _streamProcessorSuccessfullyRegistered(logger, streamProcessorId, null);

        internal static void StreamProcessorUnregistered(this ILogger logger, StreamProcessorId streamProcessorId)
            => _streamProcessorUnregistered(logger, streamProcessorId, null);
    }
}
