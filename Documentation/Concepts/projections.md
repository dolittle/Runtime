---
title: Projections
description: Overview of projections
weight: 11
---

A Projection is a special type of [Event Handler]({{< ref "event_handlers_and_filters" >}}), that only deals with updating or deleting [Read Models]({{< ref "#read-model" >}}) based on [Events]({{< ref "events" >}}) that it handles. The read model instances are managed by the [Runtime]({{< ref "overview" >}}) in a read model store, where they are fetched from whenever needed. This is useful, for when you want to create views from events, but don't want to manually manage the read model database.

Read models defines the data views that you are interested in presenting, while a projection specifies how to compute this view from the event store. There is a one-to-one relationship between a projection and their corresponding read model. A projection can produce multiple instances of that read model and it will assign each of them a unique _key_. This key is based on the projections [key selectors]({{< ref "#key-selector" >}}).

Example of a projection:

![Diagram of projections](/images/concepts/projections_v2.png)

## Read model

A read model represents a view into the data in your system, and are used when you want to show data or build a view. It's essentially a [Data transfer object](https://en.wikipedia.org/wiki/Data_transfer_object) (DTO) specialized for reading.
They are computed from the events, and are as such read-only object without any behaviour seen from the user interface.
Some also refer to read models as _materialized views_.

As read models are computed objects, you can make as many as you want based on whatever events you would like.
We encourage you to make every read model single purpose and specialized for a particular use.
By splitting up or combining data so that a read model matches exactly what an end-user sees on a single page, you'll be able to iterate on these views without having to worry how it will affect other pages.

On the other hand, if you end up having to fetch more than one read model to get the necessary data for a single page, you should consider combining those read models.

The read models are purely computed values, which you are free to throw them away or recreate lost ones at any point in time without loosing any data.

The [Runtime]({{< ref "overview" >}}) stores the read models into a read model store, which is defined in the [`resources.json`]({{< ref "docs/reference/runtime/configuration#resourcesjson" >}}). Each read model gets its own unique key, which is defined by the projections [key selector]({{< ref "#key-selector" >}}).

## Projection

A projections purpose is to populate the data structure (read model) with information from the event store. Projections behave mostly like an [event handler]({{< ref "event_handlers_and_filters" >}}), but they don't produce a [Stream]({{< ref "streams" >}}) from the events that it handles. This means that changing a projection (like adding or removing handle methods from it) will always make it replay and recalculate the read models from the start of the [Event Log]({{< ref "event_store#event-log" >}}). This makes it easier to iterate and develop these read models.

{{< alert title="Idempotence" color="warning" >}}
As changing projections will replay all of the events to it, it's very important that the handle methods of a projection are [idempotent](https://en.wikipedia.org/wiki/Idempotence#Computer_science_meaning) and only modify the read models state. A projection should not have side effects, like sending out emails as on replay those emails would be resent.
{{< /alert >}}


This is a simplified structure of a projection:
```csharp
Projection {
    ProjectionId Guid
    Scope Guid
    ReadModel type
    EventTypes EventType[]
}
```

For the whole structure of a projections as defined in protobuf, please check [Contracts](https://github.com/dolittle/Contracts/tree/master/Source/Runtime/Projections).

### Key selector

Each read model instance has a _key_, which uniquely identifies it within a projection. A projection handles multiple instances of its read models by fetching the read model with the correct key. It will then apply the changes of the `on` methods to that read model instance.

The projection fetches the correct read model instance by specifying the key selector for each `on` method. There are 3 different key selector:

- Event source based key selector, which defines the read model instances key as the events [`EventSourceId`]({{< ref "events#eventsourceid" >}}).
- Event property based key selector, which defines the key as the handled events property.
- Partition based key selector, which defines the key as the events streams [`PartitionId`]({{< ref "streams#partitions" >}}).
