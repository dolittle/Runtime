---
title: Event Sourcing
description: Overview of Event Sourcing in the Dolittle Platform
keywords: Overview, Domain Events, Event Sourcing, Event Store, Log
weight: 30
---

Event Sourcing is an approach that derives the current state of an application from the sequential [Events]({{< ref "events" >}}) that have happened within the application. These events are stored to an append-only [Event Store]({{< ref "event_store" >}}) that acts as a record for all state changes in the system.

[Events]({{< ref "events" >}}) are facts and Event Sourcing is based on the incremental accretion of knowledge about our application / domain.  Events in the [log]({{< ref "event_store#event-log" >}}) **cannot be changed or deleted**. They represent things that have happened. Thus, in the absence of a time machine, they cannot be made to un-happen.

Here's an overview of Event Sourcing:

![Basic anatomy of event sourcing](/images/concepts/eventsourcing.png)

## Problem
A traditional model of dealing with data in applications is [CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) (create, read, update, delete). A typical example is to read data from the database, modify it, and update the current state of the data. Simple enough, but it has some limitations:

- Data operations are done directly against a central database, which can slow down performance and limit scalability
- Same piece of data is often accessed from multiple sources at the same time. To avoid conflicts, transactions and locks are needed
- Without additional auditing logs, the history of operations is lost. More importantly, the [_reason_]({{< ref "#reason-for-change" >}}) for changes is lost.

### Advantages with Event Sourcing

- **Horizontal scalability**
    - With an event store, it's easy to separate change handling and state querying, allowing for easier horizontal scaling. The events and their projections can be scaled independently of each other.
    - Event producers and consumers are decoupled and can be scaled independently.
- **Flexibility**
   - The [Event Handlers]({{< ref "event_handlers_and_filters" >}}) react to events committed to the event store. The handlers know about the event and its data, but they don't know or care what caused the event. This provides great flexibility and can be easily extended/integrated with other systems.
- **Replayable state**
    - The state of the application can be recreated by just re-applying the events. This enables rollbacks to any previous point in time.
    - Temporal queries make it possible to determine the state of the application/entity at any point in time.
- **Events are natural**
    - Events are easily modeled in domain terms, avoiding [object-relational impedance mismatch](https://en.wikipedia.org/wiki/Object%E2%80%93relational_impedance_mismatch). Events are simple objects describing actions.
- **Audit log**
    - The whole history of changes is recorded in an append-only store for later auditing.
    - Instead of being a simple record of reads/writes, the [_reason_]({{< ref "#reason-for-change" >}}) for change is saved within the events.

### Problems with Event Sourcing

- **Eventual consistency**
    - As the events are separated from the projections made from them, there will be some delay between committing an event and handling it in handlers and consumers.
- **Event store is append-only**
    - As the event store is append-only, the only way to update an entity is to create a compensating event.
    - Changing the structure of events is hard as the old events still exist in the store and need to also be handled.


## Projections

The Event Store defines how the events are written in the system, it does not define or prescribe how things are read or interpreted. Committed events will be made available to any potential subscribers, which can process the events in any way they require. One common scenario is to update a read model/cache of one or multiple views, also known as a _projections_ or _materialized view_. As the Event Store is not ideal for querying data, a prepopulated view that reacts to changes is used instead. Dolittle has no built-in support for a specific style of projection as the requirements for that are out of scope of the platform.

## Compensating events

To negate the effect of an Event that has happened, another Event has to occur that reverses the effect. This can be seen in any mature Accounting domain where the Ledger is an immutable event store or journal. Entries in the ledger cannot be changed. The current balance can be derived at any point by accumulating all the changes (entries) that have been made and summing them up (credits and debts). In the case of mistakes, an explicit correcting action would be made to fix the ledger.

## Commit vs Publish

Dolittle doesn't publish events, rather they are [_committed_]({{< ref "event_store#commit-vs-publish" >}}). Events are committed to the [event log]({{< ref "event_store#event-log" >}}), from which any potential subscribers will pick up the event from and process it. There is no way to "publish" to a particular subscriber as all the events are available on the event log, but you can create a [Filter]({{< ref "event_handlers_and_filters#filter" >}}) that creates a [Stream]({{< ref "streams" >}}).

## Reason for change

By capturing all changes in the forms of events and modeling the _why_ of the change (in the form of the event itself), an Event Sourced system keeps as much information as possible.

A common example is of a e-shopping that wants to test a theory:

>
> A user who has an item in their shopping cart but does not proceed to buy it will be more likely to buy this item in the future
>

In a traditional CRUD system, where only the state of the shopping cart (or worse, completed orders) is captured, this hypothesis is hard to test. We do not have any knowledge that an item was added to the cart, then removed.

On the other hand, in an Event Sourced system where we have events like `ItemAddedToCart` and `ItemRemovedFromCart`, we can look back in time and check exactly how many people had an item in their cart at some point and did not buy it, subsequently did.  This requires no change to the production system and no time to wait to gather sufficient data.

When creating an Event Sourced system we should not assume that we know the business value of all the data that the system generates, or that we always make well-informed decisions for what data to keep and what to discard.

## Further reading
- [Martin Fowler on Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)
- [Microsoft on Event Sourcing pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
