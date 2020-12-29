---
title: Events
description: The source of truth in the system
weight: 2
repository: https://github.com/dolittle/Runtime
---

An Event is a serializable representation of _"a fact that has happened within your system"_.

## “A fact”
An event is a change (fact) within our system. The event itself contains all the relevant information concerning the change. At its simplest, an event can be represented by a name (type) if it's enough to describe the change.

More usually, it is a simple Data Transfer Object (DTO) that contains state and properties that describe the change. It does not contain any calculations or behavior.

## “that has happened” 
As the event has happened, it cannot be changed, rejected, or deleted. This forms the basis of [Event Sourcing]({{< ref "event-sourcing" >}}) If you wish to change the action or the state change that the event encapsulates, then it is necessary to initiate an action that results in another event that nullifies the impact of the first event.

This is common in accounting, for example:
Sally adds 100$ into her bank, which would result in an event like "Add 100$ to Sally's account". But if the bank accidentally adds 1000$ instead of the 100$ then a correcting event should be played, like "Subtract 900$ from Sally's account". And with event sourcing, this information is preserved in the event store for eg. later auditing purposes.

### Naming
To indicate that the event “has happened in the past”, it should be named as verb in the past tense.  Often it can contain the name of the entity that the change or action is affecting.

- ✅ `DishPrepared`
- ✅ `ItemAddedToCart`
- ❌ `StartCooking`
- ❌ `AddItemToCart`

## “within your system”
An event represents something interesting that you wish to capture in your system. Instead of seeing state changes and actions as side effects, they are explicitly modeled within the system and captured within the name, state and shape of our Event.

> State transitions are an important part of our problem space and should be modeled within our domain — Greg Young

### Naming
An event should be expressed in language that makes sense in the domain, also known as [Ubiquitous Language](https://martinfowler.com/bliki/UbiquitousLanguage.html). You should avoid overly technical/CRUD-like events where such terms are not used in the domain.

For example, in the domain of opening up the kitchen for the day and adding a new item to the menu:
- ✅ `KitchenOpened`
- ✅ `DishAddedToMenu`
- ❌ `TakeoutServerReadyForRequests`
- ❌ `MenuListingElementUpdated`

## Structure of an Event

```csharp
Event {
    Content object
    EventSourceId Guid
    EventType {
        EventTypeId Guid
        Generation int
    }
}
```

## EventSourceId
`EventSourceId` represents the source of the event like a "primary key" in a traditional database.

## EventType
An EventType is a combination of a GUID to uniquely identify the type of event it is and the event types _generation_.
This decouples the event from a programming language and enables renaming of events as the domain language evolves.
Simply put, event types are a wrapper for the actual type of your event.

```csharp
// simplified structure of an EventType
EventType {
    EventTypeId Guid
    // optional, defaults to 1
    Generation uint
}
```

The runtime doesn't know or care about the event's content, properties or type (in its respective programming language). The runtime saves the event to the event store and then filters it to the respective [EventHandlers & Filters]({{< ref "event-handlers" >}}) which exist on the client side. For this event to be accurately serialized to JSON and then deserialized back to a type that the client understands, an event type is required.

This diagram shows us a simplified view of committing a single event with the type of `DishPrepared`. The runtime receives the event, and sends it back to us to be handler. Without the event type, the SDK wouldn't know how to deserialize the JSON message coming from the runtime.

![Flow of committing an event type](/images/concepts/eventtype.png)

Event types are also important when wanting to deserialize events coming from other microservices. As the other microservice could be written in a completely different programming language, event types provide a level of abstraction for deserializing the events. Using event types 

{{< alert title="Why not use class/type names instead of GUIDs?" color="primary" >}}
When consuming events from other microservices it's important to remember that they both have their own domain and name things according to their own domain. For example what another microservice calls `CustomerRegistered` could be 
{{< /alert >}}

### Generations
As the code changes, the event type is also bound to change at some point. 

## Commit vs Publish

## Public vs. Private
There is a basic distinction between private events and public events. In much the same way that you would not grant access to other applications to your internal database, you do not allow other applications to subscribe to your private events.

Private events are only accessible within a single [Tenant]({{< ref "tenant" >}}) so that an event committed for one tenant cannot be handled outside of that tenant. Private events model the system within its domain and should be named in a way that makes sense inside the domain.

Public events are also accessible within a single tenant but they can also be added to a public [Stream]({{< ref "event-handlers" >}}) for other microservices to consume.

Your external stream of public events is your contract to the outside world, your API.

{{< alert color="primary" >}}
Extra caution should be paid to changing public events so as not to break other microservices consuming those events.
{{< /alert >}}


## Metadata
