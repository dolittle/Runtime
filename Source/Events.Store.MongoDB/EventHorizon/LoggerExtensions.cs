// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon;

static class LoggerExtensions
{
    static readonly Action<ILogger, ulong, Guid, Guid, Guid, Exception> _writingEventHorizonEvent = LoggerMessage
        .Define<ulong, Guid, Guid, Guid>(
            LogLevel.Trace,
            new EventId(340629519, nameof(WritingEventHorizonEvent)),
            "Writing event horizon event: {EventLogSequenceNumber} from tenant: {Tenant} in microservice {Microservice} to scope: {Scope}");

    static readonly Action<ILogger, ulong, Guid, Exception> _writingEventToPublisStream = LoggerMessage
        .Define<ulong, Guid>(
            LogLevel.Trace,
            new EventId(355323827, nameof(WritingEventToPublisStream)),
            "Writing event: {EventLogSequenceNumber} to public stream: {Stream}");

    internal static void WritingEventHorizonEvent(this ILogger logger, EventLogSequenceNumber eventLogSequenceNumber, TenantId producerTenant, MicroserviceId microserviceId, ScopeId scope)
        => _writingEventHorizonEvent(logger, eventLogSequenceNumber, producerTenant, microserviceId, scope, null);

    internal static void WritingEventToPublisStream(this ILogger logger, EventLogSequenceNumber eventLogSequenceNumber, StreamId stream)
        => _writingEventToPublisStream(logger, eventLogSequenceNumber, stream, null);
}