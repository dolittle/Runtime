// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

static class LoggerExtensions
{
    static readonly Action<ILogger, IStreamProcessorId, Exception> _gettingStreamProcessorState = LoggerMessage
        .Define<IStreamProcessorId>(
            LogLevel.Trace,
            new EventId(36480958, nameof(GettingStreamProcessorState)),
            "Trying to get stream processor state for: {StreamProcessorId}");

    static readonly Action<ILogger, IStreamProcessorId, Exception> _persistingStreamProcessorState = LoggerMessage
        .Define<IStreamProcessorId>(
            LogLevel.Trace,
            new EventId(1242042674, nameof(PersistingStreamProcessorState)),
            "Persisting stream processor state for: {StreamProcessorId}");

    internal static void GettingStreamProcessorState(this ILogger logger, IStreamProcessorId streamProcessor)
        => _gettingStreamProcessorState(logger, streamProcessor, null);

    internal static void PersistingStreamProcessorState(this ILogger logger, IStreamProcessorId streamProcessor)
        => _persistingStreamProcessorState(logger, streamProcessor, null);
}