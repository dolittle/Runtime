---
title: Event Store
description: The definition of the Runtime Event Store service
weight: 1
---

## Message types
<div class="mermaid">
classDiagram
    %% commit events
    class CommitEventsRequest{
        CallRequestContext callContext
        UncommittedEvent[] events
    }
    class UncommittedEvent{
        string content
        Artifact artifact
        Uuid eventSourceId
        bool public
    }
    class CommitEventsResponse{
        Failure? failure
        CommittedEvent[] events
    }
    class CommittedEvent{
        string content
        Artifact type
        Uuid eventSourceId
        bool public
        ulong eventLogSequenceNumber
        Timestamp occurred
        ExecutionContext executionContext
        bool external
        ulong? externalEventLogSequenceNumber
        Timestamp? externalEventReceived
    }
    %% commit aggregate event
    class CommitAggregateEventsRequest{
        CallRequestContext callContext
        UncommittedAggregateEvents events
    }
    class UncommittedAggregateEvents{
        Uuid aggregateRootId
        Uuid eventSourceId
        uing64 expectedAggregateRootVersion
        UncommittedAggregateEvent[] events
    }
    class UncommittedAggregateEvent{
        string content
        Artifact artifact
        bool public
    }
    class CommitAggregateEventsResponse{
        Failure failure
        CommittedAggregateEvents events
    }
    class CommittedAggregateEvents{
        Uuid aggregateRootId
        Uuid eventSourceId
        uing64 aggregateRootVersion
        CommittedAggregateEvent[] events
    }
    class CommittedAggregateEvent{
        ulong eventLogSequenceNumber
        Timestamp occurred
        ExecutionContext executionContext
        Artifact type
        bool public
        string content
    }
    %% fetch aggregate events
    class FetchForAggregateRequest{
        CallRequestContext callContext
        Aggregate aggregate
    }
    class Aggregate{
        Uuid aggregateRootId
        Uuid eventSourceId
    }
    class FetchForAggregateResponse{
        Failure? failure
        CommittedAggregateEvents events
    }
    %%
    %% commit events
    CallRequestContext *-- CommitEventsRequest
    link CallRequestContext "{{< ref "types" >}}" "Types"
    UncommittedEvent "*" o-- CommitEventsRequest
    Artifact o-- UncommittedEvent
    link Artifact "{{< ref "types" >}}" "Types"
    Failure *-- CommitEventsResponse
    link Failure "{{< ref "types" >}}" "Types"
    CommittedEvent "*" o-- CommitEventsResponse
    Artifact o-- CommittedEvent
    ExecutionContext o-- CommittedEvent
    link ExecutionContext "{{< ref "types" >}}" "Types"
    %% commit aggregate events
    UncommittedAggregateEvent *-- UncommittedAggregateEvents
    Artifact o-- UncommittedAggregateEvent
    UncommittedAggregateEvents *-- CommitAggregateEventsRequest
    CallRequestContext *-- CommitAggregateEventsRequest
    Failure *-- CommitAggregateEventsResponse
    CommittedAggregateEvents *-- CommitAggregateEventsResponse
    CommittedAggregateEvent *-- CommittedAggregateEvents
    ExecutionContext o-- CommittedAggregateEvent
    Artifact o-- CommittedAggregateEvent
    %% fetch aggrgate events
    CallRequestContext *-- FetchForAggregateRequest
    Aggregate *-- FetchForAggregateRequest
    Failure *-- FetchForAggregateResponse
    CommittedAggregateEvents *-- FetchForAggregateResponse
</div>

## Commit events
<div class="mermaid">
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: CommitEventsRequest
    activate R
    R->>C: CommitEventsResponse
    deactivate R
</div>

## Commit aggregate events
<div class="mermaid">
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: CommitAggregateEventsRequest
    activate R
    R->>C: CommitAggregateEventsResponse
    deactivate R
</div>

## Fetch aggregate events
<div class="mermaid">
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: FetchForAggregateRequest
    activate R
    R->>C: FetchForAggregateResponse
    deactivate R
</div>
