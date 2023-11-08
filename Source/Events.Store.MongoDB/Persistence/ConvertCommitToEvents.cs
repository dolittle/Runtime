// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Persistence;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Persistence;

/// <summary>
/// Represents an implementation of <see cref="IConvertCommitToEvents"/>.
/// </summary>
public class ConvertCommitToEvents : IConvertCommitToEvents
{
    /// <inheritdoc />
    public IEnumerable<Events.Event> ToEvents(Commit commit)
    {
        var eventsInCommit = commit.Events.ToArray();
        var aggregateEventsInCommit = commit.AggregateEvents.ToArray();
        if (eventsInCommit.Length == 0 && aggregateEventsInCommit.Length == 0)
        {
            return Enumerable.Empty<Events.Event>();
        }

        var eventsToStore = new List<MongoDB.Events.Event>();

        foreach (var committedEvents in eventsInCommit)
        {
            var executionContext = committedEvents.First().ExecutionContext.ToStoreRepresentation();
            eventsToStore.AddRange(committedEvents.Select(_ => new Events.Event(
                _.EventLogSequenceNumber,
                executionContext,
                new EventMetadata(
                    _.Occurred.UtcDateTime,
                    _.EventSource,
                    _.Type.Id,
                    _.Type.Generation,
                    _.Public),
                aggregate:null,
                eventHorizonMetadata:null,
                BsonDocument.Parse(_.Content))));
        }
        foreach (var committedEvents in aggregateEventsInCommit)
        {
            var executionContext = committedEvents[0].ExecutionContext.ToStoreRepresentation();
            eventsToStore.AddRange(committedEvents.Select(_ => new Events.Event(
                _.EventLogSequenceNumber,
                executionContext,
                new EventMetadata(
                    _.Occurred.UtcDateTime,
                    _.EventSource,
                    _.Type.Id,
                    _.Type.Generation,
                    _.Public),
                new AggregateMetadata(true, _.AggregateRoot.Id, _.AggregateRoot.Generation, _.AggregateRootVersion),
                null,
                BsonDocument.Parse(_.Content))));
        }
        return eventsToStore;
    }
}
