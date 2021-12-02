// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.Events.Store.Services.WebAPI;

public record FailureResponse(Guid Id, string Reason)
{
    public static FailureResponse From(Failure failure)
        => new(failure.Id, failure.Reason);
}
public record JsonResponseCommittedEvent(
    ulong EventLogSequenceNumber,
    DateTimeOffset Occurred,
    string EventSource,
    ExecutionContext ExecutionContext,
    Artifact Type,
    bool Public,
    string Content
)
{
    public static JsonResponseCommittedEvent From(CommittedEvent @event)
        => new(
            @event.EventLogSequenceNumber,
            @event.Occurred,
            @event.EventSource,
            ExecutionContext.From(@event.ExecutionContext),
            Artifact.From(@event.Type),
            @event.Public,
            @event.Content);
}

public record JsonResponseCommittedAggregateEvent(
    ulong EventLogSequenceNumber,
    DateTimeOffset Occurred,
    ExecutionContext ExecutionContext,
    Artifact Type,
    bool Public,
    string Content
)
{
    public static JsonResponseCommittedAggregateEvent From(CommittedAggregateEvent @event)
        => new(
            @event.EventLogSequenceNumber,
            @event.Occurred,
            ExecutionContext.From(@event.ExecutionContext),
            Artifact.From(@event.Type),
            @event.Public,
            @event.Content);
}

public record JsonResponseCommittedAggregateEvents(string EventSourceId, Guid AggregateRoot, ulong AggregateRootVersion, JsonResponseCommittedAggregateEvent[] Events)
{
    public static JsonResponseCommittedAggregateEvents From(CommittedAggregateEvents events)
        => new(
            events.EventSource,
            events.AggregateRoot,
            events.AsEnumerable().LastOrDefault()?.AggregateRootVersion ?? 0,
            events.ToArray().Select(_ => JsonResponseCommittedAggregateEvent.From(_)).ToArray());
    public static JsonResponseCommittedAggregateEvents From(EventSourceId eventSource, Artifacts.ArtifactId aggregateRoot)
        => new(
            eventSource,
            aggregateRoot,
            0,
            Array.Empty<JsonResponseCommittedAggregateEvent>());
}

public record CommitResponse(JsonResponseCommittedEvent[] Events, FailureResponse Failure)
{
    public static CommitResponse From(CommittedEvents events)
        => new(events.Select(_ => JsonResponseCommittedEvent.From(_)).ToArray(), null);
    public static CommitResponse From(Failure failure)
        => new(Array.Empty<JsonResponseCommittedEvent>(), FailureResponse.From(failure));
}
public record CommitForAggregateResponse(JsonResponseCommittedAggregateEvents Events, FailureResponse Failure)
{
    public static CommitForAggregateResponse From(CommittedAggregateEvents events)
        => new(JsonResponseCommittedAggregateEvents.From(events), null);
    public static CommitForAggregateResponse From(Failure failure, EventSourceId eventSource, Artifacts.ArtifactId aggregateRoot)
        => new(JsonResponseCommittedAggregateEvents.From(eventSource, aggregateRoot), FailureResponse.From(failure));
}
public record FetchForAggregateResponse(JsonResponseCommittedAggregateEvents Events, FailureResponse Failure)
{
    public static FetchForAggregateResponse From(CommittedAggregateEvents events)
        => new(JsonResponseCommittedAggregateEvents.From(events), null);
    public static FetchForAggregateResponse From(Failure failure, EventSourceId eventSource, Artifacts.ArtifactId aggregateRoot)
        => new(JsonResponseCommittedAggregateEvents.From(eventSource, aggregateRoot), FailureResponse.From(failure));
}