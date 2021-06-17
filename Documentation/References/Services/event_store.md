---
title: Event Store
description: The definition of the Runtime Event Store service
weight: 1
---

## Message types
<div class="mermaid">
classDiagram
    class Uuid{
        bytes value
    }
    class Failure{
        Uuid id
        string reason
    }
    class Artifact{
        Uuid id
        uint32 generation
    }
    class CallRequestContext{
        ExecutionContext executionContext
        Uuid headId
    }
    class ExecutionContext{
        Uuid microserviceId
        Uuid tenantId
        Version version
        Uuid correlationid
        Claim[] claims
        string environment
    }
    class Version{
        int32 major
        int32 minor
        int32 patch
        int32 build
        string preReleaseString
    }
    class Claim{
        string key
        string value
        string valueType
    }
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
    ExecutionContext *-- CallRequestContext
    UncommittedEvent "*" o-- CommitEventsRequest
    Artifact o-- UncommittedEvent
    Failure *-- CommitEventsResponse
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
