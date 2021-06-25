---
title: Filters
description: The definition of the Runtime Filters service
weight: 5
---


## Register Unpartitioned Private Filter

Registers an unpartitioned callback-type filter, to filter events from the (all tenants) event log to a stream. The runtime will call the client with events to decide if they should be in the stream.

<div class="mermaid">
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: FilterRegistrationRequest
    activate R
    alt If registration fails
        R->>C: FilterRegistrationResponse (with Failure)
    else If registration succeeds
        R->>C: FilterRegistrationResponse
        deactivate R
        loop For each event in the event log
            R->>C: FilterEventRequest
            activate C
            C->>R: FilterResponse
            deactivate C
        end
    end
</div>


### Registration message types

<div class="mermaid">
classDiagram
    class FilterRegistrationRequest{
        ReverseCallArgumentsContext callContext
        Uuid scopeId
        Uuid filterId
    }
    class FilterRegistrationResponse{
        Failure failure
    }
    %%
    FilterRegistrationRequest --* ReverseCallArgumentsContext
    FilterRegistrationResponse --o Failure
    %% links
    link ReverseCallArgumentsContext "{{< ref "types" >}}" "Types"
    link Failure "{{< ref "types" >}}" "Types"
</div>

### Event message types

<div class="mermaid">
classDiagram
    class FilterEventRequest{
        ReverseCallRequestContext callContext
        CommittedEvent event
        Uuid scopeId
        RetryProcessingState retryProcessingState
    }
    class FilterResponse{
        ReverseCallResponseContext callContext
        bool isIncluded
        ProcessorFailure failure
    }
    %%
    FilterEventRequest --* ReverseCallRequestContext
    FilterEventRequest --* CommittedEvent
    FilterEventRequest --* RetryProcessingState
    FilterResponse --* ReverseCallResponseContext
    FilterResponse --* ProcessorFailure
    %% links
    link ReverseCallRequestContext "{{< ref "types" >}}" "Types"
    link ReverseCallResponseContext "{{< ref "types" >}}" "Types"
    link RetryProcessingState "{{< ref "types#event-processing-types" >}}" "Types"
    link ProcessorFailure "{{< ref "types#event-processing-types" >}}" "Types"
    link CommittedEvent "{{< ref "event_store#message-types" >}}" "Event Store"
</div>

## Register Partitioned Private Filter

Registers a partitioned callback-type filter, to filter events from the (all tenants) event log to a stream. The runtime will call the client with events to decide if they should be in the stream, and in what partition.

# DO THIS PART

<div class="mermaid">
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: PartitionedFilterRegistrationRequest
    activate R
    alt If registration fails
        R->>C: FilterRegistrationResponse (with Failure)
    else If registration succeeds
        R->>C: FilterRegistrationResponse
        deactivate R
        loop For each event in the event log
            R->>C: FilterEventRequest
            activate C
            C->>R: PartitionedFilterResponse
            deactivate C
        end
    end
</div>


### Registration message types

<div class="mermaid">
classDiagram
    class FilterRegistrationRequest{
        ReverseCallArgumentsContext callContext
        Uuid scopeId
        Uuid filterId
    }
    class FilterRegistrationResponse{
        Failure failure
    }
    %%
    FilterRegistrationRequest --* ReverseCallArgumentsContext
    FilterRegistrationResponse --o Failure
    %% links
    link ReverseCallArgumentsContext "{{< ref "types" >}}" "Types"
    link Failure "{{< ref "types" >}}" "Types"
</div>

## Register Public Filter 

Registers a partitioned callback-type filter, to filter public events from the (all tenants) event log to a public stream. The runtime will call the client with public events to decide if they should be in the stream, and in what partition.

<div class="mermaid">
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: FetchForAggregateRequest
    activate R
    R->>C: FetchForAggregateResponse
    deactivate R
</div>

### Message types

<div class="mermaid">
classDiagram
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
    FetchForAggregateRequest --* Aggregate
    CommittedAggregateEvents --* CommittedAggregateEvent
    FetchForAggregateRequest --* CallRequestContext
    CommittedAggregateEvent --o Artifact
    CommittedAggregateEvent --o ExecutionContext
    FetchForAggregateResponse --* Failure
    FetchForAggregateResponse --* CommittedAggregateEvents
    %% links
    link CallRequestContext "{{< ref "types" >}}" "Types"
    link ExecutionContext "{{< ref "types" >}}" "Types"
    link Artifact "{{< ref "types" >}}" "Types"
    link Failure "{{< ref "types" >}}" "Types"
</div>
