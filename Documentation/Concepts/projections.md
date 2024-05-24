---
title: Projections
description: Overview of projections
weight: 11
---


In order to be able to query application state, you need to be able to read it. Since our state is stored as events, we need to project it into queryable read models.
This is what projections are good for.

Previously, projections have been a feature with special support in the runtime. This has been replaced with an client implementation which uses the event handler infrastructure under the hood.
These projections are defined as classes with methods that handle events, and are marked with the `[Projection]` attribute and extending the ReadModel base class.

The base class includes metadata such as which event updated it last, when the data was current and the key of the read model instance.

All projections are asynchronous and eventually consistent, meaning that they will be updated in the background as events are written to the event store, same as read models updated by a normal event handler.


### Example projection
```csharp
[Projection("185107c2-f897-40c8-bb06-643b3642f230")]
public class DishesEaten: ReadModel
{
    public string[] Dishes { get; set; } = {};

    public void On(DishEaten evt, ProjectionContext ctx) // Create On-methods for each event you want to handle
    {
        Dishes = Dishes.Append(evt.Dish).ToArray(); // And update its state
    }
}
```

# Usage

The system will handle the rest, including replaying the events to the projection, and storing the read model in the read model store.
In order to use the read model, you can inject `IProjectionOf<TReadModel>` into your services. It exposes methods for querying it, and also for subscribing to changes.




### Key selector

Each read model instance has a _key_, which uniquely identifies it within a projection. A projection handles multiple instances of its read models by fetching the read model with the correct key. It will then apply the changes of the `on` methods to that read model instance.

The projection fetches the correct read model instance by specifying the key selector for each `on` method. Example key selectors are:

- Default: Event source based key selector, which defines the read model instances key as the events [`EventSourceId`]({{< ref "events#eventsourceid" >}}).
- Event property based key selector, which defines the key as the handled events property.
- Partition based key selector, which defines the key as the events streams [`PartitionId`]({{< ref "streams#partitions" >}}).
- Custom key selector, which defines the key as a custom function.
- Time based key selectors, from the events timestamp. This allows bucketing the read models by time, for example by day or month.

All attributes extending IKeySelectorAttribute can be used as key selectors. The default key selector is the KeyFromEventSourceAttribute, and can be omitted for brevity.

### Example projection with key selector
```csharp
[Projection("185107c2-f897-40c8-bb06-643b3642f231")]
public class DishesEatenByCustomer: ReadModel
{
    public string[] Dishes { get; set; } = {};

    [KeyFromProperty(nameof(DishEaten.Customer))] // Use the Customer property of the DishEaten event as key / id
    public void On(DishEaten evt) // Projection context can be omitted if not needed
    {
        Dishes = Dishes.Append(evt.Dish).ToArray();
    }
}
```
