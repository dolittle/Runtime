---
title: Event Store
description: Introduction to the Event Store
keywords: Overview, Events, Event Store, Event Sourcing
author: smithmx
aliases:
    - /runtime/runtime/events/event_store
---

# Event Store

An Event Store is the mechanism by which a stream of events are persisted to durable storage.  It is the storage mechanism for an Event Sourced system (see Event Sourcing).

## Basics

There are two fundamental concepts for the Event Store:

1. A **Commit** which is a series of Events for a particular *Event Source* that is persisted as an atomic unit.
2. A **Stream** which is a series of Commits linked to a particular *Event Source*.  

{{% notice Important %}}
An Event Source is any Entity that can generate events that are persisted in a stream in the Event Store.  In DDD terms, these are most often identified as Aggregate Roots although Polices and other Event Processors can also generate Events.
{{% /notice %}}

A *Commit* is most closely associated with the concept of a *Command* and a *Command Handler* which form a *Transaction*.  A transaction operates against a single Event Source (Aggregate).  There is no explicit concept of a **Unit of Work** whereby multiple Event Streams are generated for multiple Event Sources, or where Events for multiple event sources are persisted in a single commit.  A Commit is a series of one or more events that belong to a single Event Source that are persisted in an atomic manner i.e. all events are persisted successfully or none of them are.  By focusing on the commit as the atomic unit for persistence, we avoid the need for distributed transactions or two-phase commits and enable a wider variety or storage engines for our Event Store.

A *Stream* is the sequence of Commits (and therefore of all the Events that make up these commits) that have been applied against the Event Source (e.g Aggregate Root).  By reapplying each commit and therefore each event within that commit in order, we can rehydrate an Event Source and return it to its current state.  The fact that streams are persisted against a particular Event Source allows for a basic sharding along the Event Source Id, giving greater options for scaling and performance.

## “Metadata” 

In addition to the event itself, metadata associated with the Commit and with the Event is also persisted.  

### Event Metadata

The event metadata consists of 

### Event Id.  
A unique guid for the event.
### Versioned Event Source.  
Identifies which Event Source (guid as the Id and the Artifact Id which identifies the type) and which version the Event Source is.  An Event Source is versioned individually, where each commit represents the Major Version and each event in the Commit is the minor Version.
The first commit for an Event Source is number 1.  The first event in a commit is numbered 0.
The Event Version is instrumental in supporting **optimistic concurrency**.  
### Correlation Id
A unique identifier (guid) that allows tracing of a single transaction through the system.  The correlation id identifies which transaction resulted in this commit.
### Artifact
A unique identifier for the type of the Event.  As events are long living and can evolve, they are separated from any particular system type.  An artifact identifies the concept of this event separated from any particular code representation (e.g. class).
### Original Context 
The original context contains information about the Application, Bounded Context, Tenant and Environment in which the Event was generated.  Events can be broadcast from one bounded context to another, though only one bounded context (identified by the Original Context) can own an Event.
### Occurrred
The timestamp indicating when the event occurred - when it was persisted - in UTC.

### Commit Metadata

In addition, the Commit contains an Event Store assigned, globally increasing number indicating the order in which commits were committed to the Event Store (cutting across all Event Sources).  This **Commit Sequence Number** is important in allowing *Event Processors* to track which events they have processed and to “catch up” when they re-start.

{{% notice Info %}}
Event Processors track the last version of the event that they processed in the form of the **CommittedEventVersion**.  This offset into the Event Stream is persisted.  
The CommittedEventVersion is like the EventSourceVersion mentioned above but with the CommitSequenceNumber appended in the form:
{Commit Sequence Number}.{Event Source Commit Number}.{ Event Sequence Number }
{{% /notice %}}


## Event Persistence

Events are serialized for persistence.  As an Event will have numerous code-representations over its lifetime, the Event is separated from its particular code representation and persisted as a combination of the Artifact (conceptual identification of the Event), the Generation (a number indicating which version of the Event it is) and a Property Bag that is a generic persistence mechanism for DTO like structures like the Event.  Events are intended to be serialized for persistence and communication, therefore they should be designed with this in mind (see Domain Events). You should regard the serialised version of an event as the canonical expression of it. Any run-time representation will be a reflection of this canonical version, subject to the idiosyncrasies of the particular current runtime.

## Querying the Event Store

Event Stores are not general purpose data storage engines.  As such they require and provide only rudimentary querying capabilities.

### Rehydrating an Event Source

The most common requirement to query an Event Store is to re-populate an Event Source or Aggregate Root.  This simply involves retrieving all the commits for this Event Source (identified by the Event Source Id) and re-applying them.  For performance optimisation reasons, this can be extended to include a commit version such that you only retrieve commits from a specific version number.  This is relevant when a snapshot is available that aggregates all previous events up to the specific version and you only have to apply that and subsequent commits.  When the Event Source is stateless (i.e. it does not need to keep any state to maintain its invariants) we bypass the loading of commits completely and just set the version directly.

### Catching up an Event Processor

An event processor operates at the Event rather than the Commit level, which only exists on the write-side of the event-store.  When an event processor is instantiated it checks if there have been any events of the type that it handles since the last one it handled (in the case of a new event processor this would typically be since the beginning of time).  As an optimisation the Event Store can be asked to provide all instance of a particular event type since a particular **Committed Event Version**.

### Catching up Event Horizons

Similar to Event Processors, Event Horizons have to catch up since the last version they processed.  An Event Horizon operates on Commits rather than a single event type (though it splits the commit and passes it to the appropriate Barrier).  Therefore it is possible to ask the Event Store for all Commits (across Event Sources) that have occurred since a particular version.

## Snapshots

[to be added]


## Event Migration

[to be added]