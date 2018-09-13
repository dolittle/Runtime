---
title: Domain Events
description: Description of Domain Events
keywords: Overview, Domain Events
author: smithmx
---
The key words “MUST”, “MUST NOT”, “REQUIRED”, “SHALL”, “SHALL NOT”, “SHOULD”, “SHOULD NOT”,
“RECOMMENDED”, “MAY”, and “OPTIONAL” in this document are to be interpreted as described in
[RFC 2119](https://tools.ietf.org/html/rfc2119).

## Definition

A Domain Event is a serializable representation of a fact that has happened within your domain.

The definition of a Domain Event is simple, however it has some profound implications for systems that utilise Domain Events.

## “a fact”

A Domain Event is a change (fact) within our Domain.  The Domain Event itself contains all the relevant information concerning the change. At its simplest, a Domain Event can be represented by a name (type).  This can be enough to describe the change.  More usually, it is a simple Data Transfer Object (DTO) that contains state and properties that describe the characteristics of the change.  It does not contain any calculations or behaviour.

## “that has happened” 

As it has happened, a Domain Event cannot be changed and it cannot be rejected.  Neither can you delete it.  If you wish to change the action or the state change that the Domain Event encapsulates, then it is necessary to initiate an action that results in another Domain Event that nullifies the impact of the first event.  (See Event Sourcing).

This simply means that a Domain Event is immutable.  It does not have any mutable state and does not expose any “setters”.  

To indicate the “has happened in the past” nature of a Domain Event, it should be named in form as a verb in the past tense.  Often it can contain the name of the entity that the change or action is upon.  
	•	AccountSuspended
	•	UserRegistered
	•	ItemAddedToCart

## “within your domain”

A Domain Event represents something interesting that you wish to capture in your domain.  Instead of seeing state changes and actions as side effects, they are explicitly modelled within the Domain and captured within the name, state and shape of our Event.

> State transitions are an important part of our problem space and should be modelled within our domain — Greg Young

A Domain Event should be expressed in the Ubiquitous Language of the Domain.  As such, you should avoid “crud” style events (e.g. UserUpdated, AccountCreated) where such terms are not used by the Domain Expert.


## “serialisable representation”

The concept of serialisation is inextricably linked to the concept of a Domain Event and this imposes further restrictions on the shape, form and content of our events.  The shape of your Events should be kept as simple as possible utilising only primitives or, if necessary, other DTO structures.  Events cannot contain any Entities or Aggregates.  Where required, these can be referenced by Id.  It is also strongly advised to avoid structures and types that require versioning.  You should not include anything declared within your Domain (e.g. Concepts or Value Objects) on your event as these are Type definitions that can change over time.  Similarly structures within C# such as Enums should not be included directly.  Instead the underlying primitive value should be included on the Event.  

It is possible to add simple DTOs to represent grouped data within the Event (for example, an OrderLine within an OrderPlaced event).  These must be versioned with the Event.  It is always worth considering whether you wish to take on the cognitive overhead of a DTO to version when modelling your event.

You should consider the serialisable nature of the Domain Event and let this guide the shape and content of your Event.  You should also consider the impact on versioning (see Migration).  This may seem like a departure from the principles of Domain Driven Design and the wish to capture meaning in value objects but it is in fact a recognition that an Event is primarily about communication (within and outside the Bounded Context) and persistence (in the Event Store).


### Fat Events and Thin Events

Events can be either “thin” (containing the absolute minimum information required) or “fat” (containing additional contextual information that would be useful for the consumer of the Event).  Using our example of an OrderPlaced event, a thin event would basically tells us which order had been placed and when.  It would rely on other previous events such as OrderLineAdded to create the state of the Order that was being placed.  On the other hand, a “fat” event could contain a collection of OrderLine DTOs as part of the Event so that the consumer can see that an order with the included state was placed.

In general, you should prefer thin over fat events within a bounded context.  Extra contextual information can create couplings and versioning issues.

### Metadata

An Event has a lot of common contextual information that is useful in the tracing and processing of the Event.  This metadata includes which Entity / Aggregate the event relates to, the User or System that caused the event, a Correlation id that allows the Event to be related to a Command or other action within the system, a UTC timestamp indicating when the Event occurred, the version of the Event and Aggregate, the Source (Application, Bounded Context, Tenant) and so on.  This combination of the Event and Contextual metadata is often referred to as the EventEnvelope.  The metaphor being that the metadata is the envelope and address and so on which the event is that actual contents of the letter.

Dolittle supports such Event Envelopes.  It is therefore redundant to include this contextual information on your Events.  It also draws attention away from the important aspects of the Event (the state change).




