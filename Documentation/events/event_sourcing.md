---
title: Event Sourcing
description: Overview of Event Sourcing
keywords: Overview, Domain Events, Event Sourcing, Event Store, Log
author: smithmx
aliases:
    - /runtime/runtime/events/event_sourcing
---

## Definition

Event Sourcing is an approach that derives the current state of an application from the sequential, incemental changes (events) that have
happened within the application. 

## Managing State

While Event Sourcing is a fringe approach for maintaining state within application development, it has a long history within mature business domains, especially where there is a requirement to have a dependable audit log (e.g. finance/accounting).  The traditional approach to managing state involves storing the current state of a particular record (in a database table or document).  This may be supplemented by an audit table that records changes in the data for the particular entity.  The source of truth is the current state in the table, not the audit log. 

Event Sourcing turn this approach on its head. Rather than storing the current state it advocates storing the changes / deltas in the form of immutable events in the seqeuence that they happened.  It is then possible to derive the state of any particular entity be replaying all the changes that have happened to this entity.  In an event sourced system the event log and current state are one and the same.  There is no possibility that they can be out of sync (and, being immutable, there is no possibility that your log is incorrect).  By explicitly capturing and recording state changes (deltas) as Events (see Domain Events), we can know how and why our system is in the state it is.  This is often enormously valueable to the business (see The Business Value of the Event Log).

## An immutable log

The Event Store is an *immutable, append-only log* that is the single authoritative source of truth for the system.  

Events are facts and Event Sourcing is based on the incremental accretion of knoweldge about our application / domain.  Events in the log **cannot be changed**.  They **cannot be deleted**.  They represent things that have happened. Thus, in the absence of a time machine, they cannot be made to un-happen. Do, or do not. There is no try. 

To negate the effect of an Event that has happened, it is necessary for another Event to occur that reverses the effect.  This can be clearly seen in any mature Accounting domain where the Ledger is an immutable event store or journal.  Entries in the ledger cannot be changed.  The current balance can be derived at any point but accumulating all the changes (entries) that have been made and summing the pluses and minuses (credits and debets). It would be naïve to assume that mistakes will not be made, and accountants are anything but naïve. In fact they treat such cases explictly, by making corrections explict actions.

Where a mistake has been made, a new entry in the ledger is made with an adjustment (plus or minus) to correct the mistake.  Where a mistake is made in the correction, you similarly cannot change the correction.  You must make another entry to correct the correction, and so on.  Thus we have a definitive and irrefutable record of the state of the system.  

An Event Sourced system not only knows the current state of the system, it knows how we arrived at that state.  Furthermore, it knows the state of the system at any point in time and can also tell us what the state should have been compared to what we thought it was at any particular point in time. As an aside - a *Blockchain* operates on the same principle of an immutable ledger as an Event Store, but it does so in a distributed no-trust manner.

## Don't throw away information

By capturing all changes in the forms of deltas (persisted Events) and modelling the *why* of the change (in the form of the Event itself), and Event Sourced system keeps as muc information as possible.  We do not assume that we know all the information that is or will be useful in the future at the current time.  In a traditional system that captures only the current state, only explicitly captured state can be interrogated.  If we wish to test a hypothesis, it is often necessary to make a change to the production system to capture new state, wait enough time to gather sufficient data and then test our hypothesis.  In an Event Sourced system, it is often the case that the hypothesis can be tested just by interrogating the events.

The oft-cited example of this is in the e-shopping domain and the theory that a user who has an item in their shopping cart but does not proceed to buy it will be more likely to buy this item in the future.  The traditional approach that, at best, captures only the state of the shopping cart and at worst only captures orders, cannot test this hypothesis at all.  If we do not know how the shopping cart changed, we do not know who added the item then removed it.  We do not know who emptied their cart of all items.  We might be able to tell what was in a cart that failed to convert to an order.

On the other hand, in an Event Sourced system where we have *ItemAddedToCart* and *ItemRemovedFromCart* events, we can look back in time and check exactly how many people who had an item in their cart at some point and did not buy it, subsequently did.  This requires no change to the production system and no time to wait to gather sufficicent data.

When creating an Event Sourced system we should not assume that we know the business value of all the data that the system generates for now and all time or that we always make well informed decisions for what data to keep and what to discard.

Of course, if the data is not captured in an Event or if several distinct things we would like to know are captured and folded into a single event, then that data may be lost (or only inferred rather than explicitly known).

[Diagram showing the state of the Shopping Cart]

## Single source of truth, multiple views

The append only log of events is the single source of truth in an Event Sourced system. The events are persisted in serialized form.

The Event Store defines how the events are written in the system, it does not define or prescribe how things are read or interpreted.  Events will be published from the Event Store and can be handled by subscribers to these events.  These subscribers can then process the events in any way they require. This separation of the events (what happened) from their effect (how the happenings were dealt with) is the core of Event Sourcing. Decoupling this allows deeper knowledge gained to be applied to previously captured state-changes.

A typical scenario will involve updating a materialized (cached) view on the read-side of a CQRS system.  Another might be to populate the data for a Machine Learning algorithm or to feed data to an OLAP cube.  It might feed into a relational database or a graph database.  The subscribers and what they do will be determined by the requirements of the system.  The Event Sourced nature of the system imposes no limits on the subscribers / read side of the system beyond that the Event Store is the single source of truth.

## Saving events

In a traditional, state-based system, the persisting of an object or a graph of objects (an aggregate) can be a complex and involved process.  The impedance-mismatch between the object and relational realms and the complexity of ORM (Object Relational Mappers) is well known.  A common pattern with a sophisticated ORMs such as NHibernate or Entity Framework is to utilize the Unit of Work pattern: a session is opened, various objects are tracked and changed and the framework then figures out what has changed and how to update the database.  This makes the concepts of state changes / deltas implicit and hidden from the developer.  By contrast, since an Event Sourced System is based on making state changes explicit, you have a single method of persisting changes.  You write a series of events (deltas) that constitute a transaction in a single commit.  The transaction is explicit, the changes are explicit and there is no need to derive the changes by comparing a before and after object graph. We consider this to be orders of magnitudes simpler, both technically and conceptually.

## Rehydrating objects from events

When loading an object an Event Sourced system is simpler than a state based system (with or without tooling support in the form of an ORM).  When using Event Sourcing, events are associated with a particular Aggregate (see Aggregates and Aggregate Roots) and the appropriate events are simply loaded and then replayed in the order that they happened.  When the system is using the CQRS division between the read side and the write side (and based on DDD principles of an Aggregate Root) we have a single method of retrieving an aggregate.  Since the Aggregate does not expose any state and is not used for any read purposes, there is no need to consider lazy or eager loading that is required in an ORM single-model system. The Aggregate Root will be created, and it's state will be recreated at run-time by applying all changes to it directly. This may sound tremendously ineffective, but in reality it is extremely quick and efficient for most Aggregate Roots.

### Stateless objects

It would be unecessary to go through event replay when creating an aggregate root if the events don't change it. The aggregate only holds the state that it requires to maintain its invariants and ensure transactional integrity within the aggregate. In other words it only "cares" about events that affect it.  If information from an event is not involved in this process, it is not necessary for an aggregate to hold this information in state.  If an aggregate does not hold *any* state, it is not necessary to replay events to get back to the *current* state and this step can be optimized away.  Dolittle will check whether an aggregate is effectively stateless (i.e. that it has methods to reapply events) before retrieving the events from the event store.

### Snapshots

While retrieving events to replay is a simple process, since events are a cumulative process, the number of events to be retrieved is eternally and unboundedly growing.  This has obvious potential performance implications for aggregates with a large number of events.

{{% alert %}} An Event Sourced System, as it is fundamentally sharded on Aggregate Id, is more suited to partition and scale than a traditional relational database system

To improve the performance of an individual aggregate, it is *recommended* to implement the **memento pattern** through the medium of rolling snapshots.  In effect, a snapshot captures the state at a particular time and store this.  To rehydrate your object, you load the snapshot then reapply all the events that have occurred since the snapshot was made.  There are many strategies that can be used to decide when to snapshot state. Snapshots can also be discarded and rebuilt as required.
