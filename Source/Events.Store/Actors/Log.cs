// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.Actors;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Warning, "Aggregate root '{AggregateRootId}' with event source '{EventSourceId}' has version inconsistency in cache. Expected {ExpectedAggregateRootVersion} but current is {CurrentAggregateRootVersion}")]
    internal static partial void AggregateRootVersionCacheInconsistency(this ILogger logger, ArtifactId aggregateRootId, EventSourceId eventSourceId, AggregateRootVersion expectedAggregateRootVersion, AggregateRootVersion currentAggregateRootVersion);
    
    [LoggerMessage(0, LogLevel.Warning, "Aggregate root '{AggregateRootId}' with event source '{EventSourceId}' version inconsistency resolved it self. Aggregate root version is at the expected version {ExpectedAggregateRootVersion}")]
    internal static partial void AggregateRootVersionCacheInconsistencyResolved(this ILogger logger, ArtifactId aggregateRootId, EventSourceId eventSourceId, AggregateRootVersion expectedAggregateRootVersion);
    
    [LoggerMessage(0, LogLevel.Warning, "Aggregate root '{AggregateRootId}' with event source '{EventSourceId}' version has a concurrency conflict, expected version {ExpectedAggregateRootVersion} but cached version {CachedVersion} was not consistent with stored version {StoredVersion}. Updating cache")]
    internal static partial void AggregateRootConcurrencyConflictWithInconsistentCache(this ILogger logger, ArtifactId aggregateRootId, EventSourceId eventSourceId, AggregateRootVersion expectedAggregateRootVersion, AggregateRootVersion cachedVersion, AggregateRootVersion storedVersion);
    
    [LoggerMessage(0, LogLevel.Warning, "Aggregate root '{AggregateRootId}' with event source '{EventSourceId}' version has a concurrency conflict, expected version {ExpectedAggregateRootVersion} but cached version {CachedVersion} is consistent with storage")]
    internal static partial void AggregateRootConcurrencyConflictWithConsistentCache(this ILogger logger, ArtifactId aggregateRootId, EventSourceId eventSourceId, AggregateRootVersion expectedAggregateRootVersion, AggregateRootVersion cachedVersion);
}
