---
title: Projections
description: Overview of projections
weight: 11
---

After having made the switch to building event-sourced systems, you might wonder how you are supposed to get the interesting data back out of the event store.
You might think; wouldn't it be terribly inefficient and slow to loop through all the events every time I need to calculate a value for the user interface?
If that's what you're thinking, you're absolutely right!

This is where read models and projections come to the rescue.
Read models defines the data views that you are interested in presenting, while a projection specifies how to compute this view from the event store.
They are closely linked, in fact so closely that there is a one-to-one relationship between projections and their corresponding read models.

## Read models
A read model represents a view into the data in your system, and are used when you want to show data or build a view. It's essentially a [Data transfer object](https://en.wikipedia.org/wiki/Data_transfer_object) (DTO) specialized for reading.
They are computed from the events, and are as such read-only object without any behaviour seen from the user interface.
Some also refer to read models as _materialized views_.

As read models are computed objects, you can make as many as you want based on whatever events you would like.
In fact; we encourage you to make every read model single purpose and specialized for a particular use.
By splitting up or combining data so that a read model matches exactly what an end-user sees on a single page, you'll be able to iterate on these views without having to worry how it will affect other pages.

On the other hand, there is such a thing as too many read models.
If you end up having to fetch more than one read model to get the necessary data for a single page, consider it the event-sourcing spirits' way of telling you that those read models should be combined.

It's going to be a little tricky to get these read models right the first time around.
But don't worry, since they are purely computed values you are free to throw them away or recreate lost ones at any point in time without loosing any data.
So feel free to experiment and iterate until you hit the sweet-spot.


The [Runtime]({{< ref "overview" >}}) stores the read models into a read model store




## Projections
Once you've decided on the structure for a read model; you're halfway there.
The next step is to populate the data structure with information from the event store.
This is exactly and completely what projections are for.

A Projection is a special type of [Event Handler]({{< ref "event_handlers_and_filters" >}}), that only deals with updating or deleting [Read Models]({{< ref "#read-model" >}}) based on [Events]({{< ref "events" >}}) that it handles. The read model instances are managed by the [Runtime]({{< ref "overview" >}}) in a read model store (Projection Store), where they are fetched from whenever needed. This is useful, for when you want to create views from events, but don't want to manually manage the read model database.


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



A projection is composed of a [Filter]({{< ref "event_handlers_and_filters#filters" >}}) and a
