// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates
{
    static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, Guid, Exception> _incrementingVersionForAggregate = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Trace,
                new EventId(422590197, nameof(IncrementingVersionForAggregate)),
                "Incrementing version for aggregate root: {AggregateRoot} and event source: {EventSource}");

        static readonly Action<ILogger, Guid, Guid, Exception> _fetchingVersionFor = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Trace,
                new EventId(422590197, nameof(FetchingVersionFor)),
                "Fetching version for aggregate root: {AggregateRoot} and event source: {EventSource}");

        internal static void IncrementingVersionForAggregate(this ILogger logger, ArtifactId aggregateRoot, EventSourceId eventSource)
            => _incrementingVersionForAggregate(logger, aggregateRoot, eventSource, null);

        internal static void FetchingVersionFor(this ILogger logger, ArtifactId aggregateRoot, EventSourceId eventSource)
            => _fetchingVersionFor(logger, aggregateRoot, eventSource, null);
    }
}