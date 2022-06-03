// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Extension methods for <see cref="Exception" />.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Converts the <see cref="Exception" /> to a <see cref="Failure" />.
    /// </summary>
    /// <param name="exception">The <see cref="Exception" /> to convert from.</param>
    /// <param name="fullException">Whether to extract the whole exception or just the message.</param>
    /// <returns>The converted <see cref="Failure" />.</returns>
    public static Failure ToFailure(this Exception exception, bool fullException = false)
        => exception switch
        {
            EventStoreUnavailable e => new Failure(EventStoreFailures.EventStoreUnavailable, ExtractExceptionMessage(e, fullException)),
            EventWasAppliedByOtherAggregateRoot e => new Failure(EventStoreFailures.EventAppliedByOtherAggregateRoot, ExtractExceptionMessage(e, fullException)),
            EventWasAppliedToOtherEventSource e => new Failure(EventStoreFailures.EventAppliedToOtherEventSource, ExtractExceptionMessage(e, fullException)),
            EventStorePersistenceError e => new Failure(EventStoreFailures.EventStorePersistanceError, ExtractExceptionMessage(e, fullException)),
            EventStoreConsistencyError e => new Failure(EventStoreFailures.EventStoreConsistencyError, ExtractExceptionMessage(e, fullException)),
            EventLogSequenceIsOutOfOrder e => new Failure(EventStoreFailures.EventLogSequenceIsOutOfOrder, ExtractExceptionMessage(e, fullException)),
            EventCanNotBeNull e => new Failure(EventStoreFailures.EventCannotBeNull, ExtractExceptionMessage(e, fullException)),
            AggregateRootVersionIsOutOfOrder e => new Failure(EventStoreFailures.AggregateRootVersionOutOfOrder, ExtractExceptionMessage(e, fullException)),
            AggregateRootConcurrencyConflict e => new Failure(EventStoreFailures.AggregateRootConcurrencyConflict, ExtractExceptionMessage(e, fullException)),
            NoEventsToCommit e => new Failure(EventStoreFailures.NoEventsToCommit, ExtractExceptionMessage(e, fullException)),
            _ => new Failure(FailureId.Other, $"Error: {ExtractExceptionMessage(exception, fullException)}")
        };

    static string ExtractExceptionMessage(Exception ex, bool fullException) => fullException ? ex.ToString() : ex.Message;
}
