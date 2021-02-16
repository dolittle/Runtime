// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Services
{
    /// <summary>
    /// Extension methods for <see cref="Exception" />.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Converts the <see cref="Exception" /> to a <see cref="Failure" />.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to convert from.</param>
        /// <returns>The converted <see cref="Failure" />.</returns>
        public static Failure ToFailure(this Exception exception)
            => exception switch
                {
                    EventStoreUnavailable e => new Failure(EventStoreFailures.EventStoreUnavailable, e.Message),
                    EventWasAppliedByOtherAggregateRoot e => new Failure(EventStoreFailures.EventAppliedByOtherAggregateRoot, e.Message),
                    EventWasAppliedToOtherEventSource e => new Failure(EventStoreFailures.EventAppliedToOtherEventSource, e.Message),
                    EventStorePersistenceError e => new Failure(EventStoreFailures.EventStorePersistanceError, e.Message),
                    EventStoreConsistencyError e => new Failure(EventStoreFailures.EventStoreConsistencyError, e.Message),
                    EventLogSequenceIsOutOfOrder e => new Failure(EventStoreFailures.EventLogSequenceIsOutOfOrder, e.Message),
                    EventCanNotBeNull e => new Failure(EventStoreFailures.EventCannotBeNull, e.Message),
                    AggregateRootVersionIsOutOfOrder e => new Failure(EventStoreFailures.AggregateRootVersionOutOfOrder, e.Message),
                    AggregateRootConcurrencyConflict e => new Failure(EventStoreFailures.AggregateRootConcurrencyConflict, e.Message),
                    NoEventsToCommit e => new Failure(EventStoreFailures.NoEventsToCommit, e.Message),
                    _ => new Failure(FailureId.Other, $"Error message: {exception.Message}\nStack Trace: {exception.StackTrace}")
                };
    }
}
