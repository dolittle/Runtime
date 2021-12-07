// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams;

static class LoggerExtensions
{
    static readonly Action<ILogger, ulong, Guid, Guid, Exception> _writingEventToStream = LoggerMessage
        .Define<ulong, Guid, Guid>(
            LogLevel.Trace,
            new EventId(203084185, nameof(WritingEventToStream)),
            "Writing event: {EventLogSequenceNumber} to stream: {Stream} in scope: {Scope}");

    internal static void WritingEventToStream(this ILogger logger, EventLogSequenceNumber eventLogSequenceNumber, StreamId stream, ScopeId scope)
        => _writingEventToStream(logger, eventLogSequenceNumber, stream, scope, null);

}