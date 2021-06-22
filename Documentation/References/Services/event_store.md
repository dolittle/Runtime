---
title: Event Store
description: The definition of the Runtime Event Store service
weight: 1
---

## Message types
<div class="mermaid">
classDiagram
    class CommitEventsRequest{
        CallRequestContext callContext
        UncommittedEvent[] events
    }
    class UncommittedEvent{
        string content
        Artifact artifact
        Uuid eventSourceId
        bool Public
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
        DateTime occurred
        ExecutionContext executionContext
        bool external
        ulong? externalEventLogSequenceNumber
        DateTime? externalEventReceived
    }
    CallRequestContext *-- CommitEventsRequest
    UncommittedEvent "*" o-- CommitEventsRequest
    Artifact o-- UncommittedEvent
    link Artifact "{{< ref "types" >}}" "IM a link lol"
    Failure *-- CommitEventsResponse
    link Failure "{{< ref "types" >}}" "IM a link lol"
    CommittedEvent "*" o-- CommitEventsResponse
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
