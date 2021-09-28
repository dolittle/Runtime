---
title: Embeddings
description: The definition of the Runtime Embedding service
weight: 25
---

## Register Embedding

Registers a callback-type embedding, to create events from changes to a read model in an external system. The Runtime will call the client with read model states to compare and the client returns events that capture the differences. The Runtime will then call the client again with these events to update the internal state.

```mermaid
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: EmbeddingRegistrationRequest
    activate R
    alt If registration fails
        R->>C: EmbeddingRegistrationResponse (with Failure)
    else If registration succeeds
        R->>C: EmbeddingRegistrationResponse
        deactivate R
        loop
            alt For each event committed by the embedding
                R->>C: EmbeddingProjectRequest
                activate C
                C->>R: EmbeddingProjectResponse
                deactivate C
            else While projected state is not equal to the desired state
                R->>C: EmbeddingCompareRequest
                activate C
                C->>R: EmbeddingCompareResponse
                deactivate C
            else While projected state is not deleted and desired state is deleted
                R->>C: EmbeddingDeleteRequest
                activate C
                C->>R: EmbeddingDeleteResponse
                deactivate C
            end
        end
    end
```


### Registration message types

```mermaid
classDiagram
    class EmbeddingRegistrationRequest{
        ReverseCallArgumentsContext callContext
        Uuid embeddingId
        Artifact[] events
        string initialState
    }
    class EmbeddingRegistrationResponse{
        Failure failure
    }
    %%
    EmbeddingRegistrationRequest --* ReverseCallArgumentsContext
    EmbeddingRegistrationRequest --* Artifact
    EmbeddingRegistrationResponse --o Failure
    %% links
    link ReverseCallArgumentsContext "{{< ref "types" >}}" "Types"
    link Artifact "{{< ref "types" >}}" "Types"
    link Failure "{{< ref "types" >}}" "Types"
```

### Event message types

```mermaid
classDiagram
    class EmbeddingProjectRequest{
        ReverseCallRequestContext callContext
        UncommittedEvent event
        ProjectionCurrentState currentState
    }
    class EmbeddingProjectResponse{
        ReverseCallResponseContext callContext
        EmbeddingProjectResponseAction response
        ProcessorFailure failure
    }
    class EmbeddingProjectResponseAction{
        <<abstract>>
    }
    class EmbeddingCompareRequest{
        ReverseCallRequestContext callContext
        ProjectionCurrentState projectionState
        string entityState
    }
    class EmbeddingCompareResponse{
        ReverseCallResponseContext callContext
        UncommittedEvent[] event
    }
    class EmbeddingDeleteRequest{
        ReverseCallRequestContext callContext
        ProjectionCurrentState projectionState
    }
    class EmbeddingDeleteResponse{
        ReverseCallResponseContext callContext
        UncommittedEvent[] event
    }
    %%
    EmbeddingProjectRequest --* ReverseCallRequestContext
    EmbeddingProjectRequest --* ProjectionCurrentState
    EmbeddingProjectRequest --* UncommittedEvent
    EmbeddingProjectResponse --* ReverseCallResponseContext
    EmbeddingProjectResponse --* EmbeddingProjectResponseAction
    EmbeddingProjectResponse --* ProcessorFailure
    ProjectionReplaceResponse ..|> EmbeddingProjectResponseAction
    ProjectionDeleteResponse ..|> EmbeddingProjectResponseAction
    EmbeddingCompareRequest --* ReverseCallRequestContext
    EmbeddingCompareRequest --* ProjectionCurrentState
    EmbeddingCompareResponse --* ReverseCallResponseContext
    EmbeddingCompareResponse --* UncommittedEvent
    EmbeddingDeleteRequest --* ReverseCallRequestContext
    EmbeddingDeleteResponse --* ReverseCallResponseContext
    EmbeddingDeleteRequest --* ProjectionCurrentState
    EmbeddingDeleteResponse --* UncommittedEvent
    %% links
    link ReverseCallRequestContext "{{< ref "types" >}}" "Types"
    link ReverseCallResponseContext "{{< ref "types" >}}" "Types"
    link ProcessorFailure "{{< ref "types#event-processing-types" >}}" "Types"
    link UncommittedEvent "{{< ref "event_store#event-message-types" >}}" "Event Store"
    link ProjectionCurrentState "{{< ref "projections#event-message-types" >}}" "Projections"
    link ProjectionReplaceResponse "{{< ref "projections#event-message-types" >}}" "Projections"
    link ProjectionDeleteResponse "{{< ref "projections#event-message-types" >}}" "Projections"
```

## Compare Embedding by key

Triggers a comparison for the specified embedding and its unique key - for one tenant. The Runtime will call the embedding to compare the internal and desired state, and return events to commit, which in turn will update the internal state. The Runtime will reply with success if the embedding read model reached the desired state, or failure if it was impossible to reach the desired state.


## Delete Embedding by key

Triggers a delete request for the specified embedding and its unique key - for one tenant. The Runtime will call the embedding to delete the internal state, and return events to commit, which in turn will delete the internal state. The Runtime will reply with success if the embedding  read model was deleted, or failure if it was impossible to delete.


## Get one Embeddding by key

Retrieves an instance of the internal read model for a specified embedding and its unique key - for one tenant. If the instance does not exist, the  Runtime will reply with the initial state for the embedding.

```mermaid
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: GetOneRequest
    activate R
    R->>C: GetOneResponse
    deactivate R
```

### Message types

```mermaid
classDiagram
    class GetOneRequest{
        CallRequestContext callContext
        Uuid scopeId
        Uuid projectionId
        string key
    }
    class GetOneResponse{
        Failure? failure
        ProjectionCurrentState currentState
    }
    %%
    GetOneRequest --* CallRequestContext
    GetOneResponse --* ProjectionCurrentState
    GetOneResponse --* Failure
    %% links
    link ProjectionCurrentState "{{< ref "projections#event-message-types" >}}" "Types"
    link CallRequestContext "{{< ref "types" >}}" "Types"
    link Failure "{{< ref "types" >}}" "Types"
```

## Get all Embeddings

Retrieves all instances of internal read models for a specified embedding - for one tenant.

```mermaid
sequenceDiagram
    participant C as Client (SDK)
    participant R as Runtime
    C->>R: GetAllRequest
    activate R
    R->>C: GetAllResponse
    deactivate R
```

### Message types

```mermaid
classDiagram
    class GetAllRequest{
        CallRequestContext callContext
        Uuid scopeId
        Uuid projectionId
    }
    class GetAllResponse{
        Failure? failure
        ProjectionCurrentState[] currentState
    }
    %%
    GetAllRequest --* CallRequestContext
    GetAllResponse --* ProjectionCurrentState
    GetAllResponse --* Failure
    %% links
    link ProjectionCurrentState "{{< ref "projections#event-message-types" >}}" "Types"
    link CallRequestContext "{{< ref "types" >}}" "Types"
    link Failure "{{< ref "types" >}}" "Types"
```

## Get all Embedding keys

Retrieves all internal read model keys for a specified embedding - for one tenant.
