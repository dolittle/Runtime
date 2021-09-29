---
title: Embeddings
description: Overview of embeddings
weight: 13
---

Embeddings are like a combination of an [Aggregate]({{< ref "aggregates" >}}) and a [Projection]({{< ref "projections" >}}). They have a collection of states (the [read models]({{< ref "#read-model" >}})) with unique [keys]({{< ref "#key" >}}) (within the embedding). Whenever a new updated state is pushed/pulled from the external system, that updated state will be compared against its representative read model in the embedding. Whenever the states differ, the Runtime will call the embedding to resolve the difference into events. The embedding will then handle these events and modify its state to match the desired state.

The main point of embeddings is event source the changes coming from an external system. An embeddings read model exists so that we can commit the correct events and uphold its logic. Other [event handlers]({{< ref "event_handlers_and_filters" >}}), [projections]({{< ref "projections" >}}) and [microservices]({{< ref "event-horizon" >}}) can then build upon these events.

Example of an embedding:

![Diagram of embeddings](/images/concepts/embeddings.png)

## Read model

An embeddings read model functions like the state of an [Aggregate Root]({{< ref "aggregates" >}}). It upholds the invariants needed to produce the events to steer the read model towards the updated state. Whenever the read model is asked to update, it should resolve the difference between its own state and the updated state into events. These events are then projected into the read model, like a [projection]({{< ref "projections" >}}).

The read model instances are managed by the [Runtime]({{< ref "overview" >}}) in the embedding store defined by [`resources.json`]({{< ref "docs/reference/runtime/configuration#resourcesjson" >}}).

### Key

Each read model has its own unique key, which is defined in the update call to the embedding. 

Unlike projections, embeddings don't need [`KeySelectors`]({{< ref "projections#keys-elector" >}}). The Runtime will automatically calculate a unique [`EventSourceId`]({{< ref "events#eventsourceid" >}}) for the committed [events]({{< ref "#embedding-events" >}}) based on the key. This `EventSourceId` is then used to uniquely identify which read model should be handling the event.

## Embedding events

The committed events are always [public]({{< ref "events#public-vs-private" >}}) [Aggregate events]({{< ref "aggregates#aggregateevents" >}}). The `AggregateRootId` is the same as the `EmbeddingId`, and the `EventSourceId` is computed from the read models key. This way each event can be uniquely identified to been originated by a specific read model and embedding.

You can't "replay" an embeddings projection, and you shouldn't need to. If you need the embedding to be in a specific state, you ask it to update itself to match the desired state. As each event of an embedding is specific to a key, embedding and the version of the embedding, you can't "re-apply" those events to the embedding. If the read model and events need to change, you should create a new embedding to handle that.

{{< alert title="Committing embeddings events outside of embedding" color="warning" >}}
Only the embedding can commit the types of events that it handles, so that it's the only source of those particular event types. This is because the embedding is trying to event source the state of the external system, which is the real "source of truth". If the embedding would start receiving events coming from somewhere else it would be as another competing "source of truth" had appeared.

To enforce this, the embedding works like an [Aggregate Root]({{< ref "aggregates" >}}) and keeps track of how many events it has handled and what is the events expected [`AggregateRootVersion`]({{< ref "aggregates#version" >}}).
{{< /alert >}}

## Persistence

The embeddings definition is persisted in the `embedding-definition` collection in the database defined in [`resources.json`]({{< ref "docs/reference/runtime/configuration#resourcesjson" >}}). If this definition changes in some way (eg. new event types, different initial state) the registration will fail as the embedding isn't the same embedding anymore.

The embeddings current [`AggregateRootVersion`]({{< ref "aggregates#version" >}}) is saved to the [`aggregates`]({{< ref "event_store#aggregates" >}}) collection.

Embeddings don't produce [streams]({{< ref "streams" >}}). 

## Main structure of an Embedding

This is a simplified structure of an embedding:
```csharp
Embedding {
    EmbeddingId Guid
    ReadModel type
    EventTypes EventType[]
}
```

For the whole structure of an embedding as defined in protobuf, please check [Contracts](https://github.com/dolittle/Contracts/tree/master/Source/Runtime/Embeddings).
