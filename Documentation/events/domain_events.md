---
title: Domain Events
description: Description of Domain Events
keywords: Overview, Domain Events
author: smithmx
aliases:
    - /runtime/runtime/events/domain_events
---

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

{{% alert %}}
Dolittle requires that the event properties are read-only and match the name of constructor-parameters. The constructor-parameter names must start with a small letter, while the property-names must start with a capital letter.

Thus, if you have a property "UnitPrice" it must match a constructor-parameter "unitPrice" of the same type.
{{% /alert %}}

```csharp
public class ItemAddedToCart(AddRecommendationToCart cmd)
{
	public ItemAddedToCart(Guid item, int quantity, decimal unitPrice, decimal lineTotal, string currency)
	{
		Item = item;
		Quantity = quantity;
		UnitPrice = unitPrice;
		LineTotal = lineTotal;
		Currency = currency;
	}

	Guid Item { get; };
	int Quantity { get; }
	decimal UnitPrice { get; }
	decimal LineTotal { get; }
	string Currency { get; }
}
```

It is possible to add simple DTOs to represent grouped data within the Event (for example, an OrderLine within an OrderPlaced event).  These must be versioned with the Event.  It is always worth considering whether you wish to take on the cognitive overhead of a DTO to version when modelling your event.  Taking our example above, we could (though it is not recommended) create an Amount DTO that combines the total and currency.

```csharp
public class ItemAddedToCart(AddRecommendationToCart cmd)
{
	public ItemAddedToCart(Guid item, int quantity, Amount unitPrice, Amount lineTotal)
	{
		Item = item;
		Quantity = quantity;
		UnitPrice = unitPrice;
		LineTotal = lineTotal;
	}

	Guid Item { get; };
	int Quantity { get; }
	Amount UnitPrice { get; }
	Amount LineTotal { get; }
}

public class Amount 
{
	public Amount(decimal total, string currency)
	{
		Total = total;
		Currency = currency;
	}

	decimal Total { get; }
	string Currency { get; }
}
```

An alternative approach would be to combine the total and currency into a single atomic value:

```csharp
public class ItemAddedToCart(AddRecommendationToCart cmd)
{
	public ItemAddedToCart(Guid item, int quantity, string unitPrice, string lineTotal)
	{
		Item = item;
		Quantity = quantity;
		UnitPrice = unitPrice;
		LineTotal = lineTotal;
	}

	Guid Item { get; };
	int Quantity { get; }
	string UnitPrice { get; }
	string LineTotal { get; }
}

//usage
var added = new ItemAddedToCart(Guid.NewGuid(),1,"500 EUR", "500 EUR")

```

You should consider the serialisable nature of the Domain Event and let this guide the shape and content of your Event.  You should also consider the impact on versioning (see Migration) and any logic (e.g. parsing as in the above example).  This may seem like a departure from the principles of Domain Driven Design and the wish to capture meaning in value objects but it is in fact a recognition that an Event is primarily about communication (within and outside the Bounded Context) and persistence (in the Event Store).


### Fat Events and Thin Events

Events can be either “thin” (containing the absolute minimum information required) or “fat” (containing additional contextual information that would be useful for the consumer of the Event).  Using our example of an OrderPlaced event, a thin event would basically tells us which order had been placed and when.  It would rely on other previous events such as OrderLineAdded to create the state of the Order that was being placed.  On the other hand, a “fat” event could contain a collection of OrderLine DTOs as part of the Event so that the consumer can see that an order with the included state was placed.

In general, you should prefer thin over fat events within a bounded context.  Extra contextual information can create couplings and versioning issues.  For events that are primarily intended for integration with other bounded context, fat events may be more suitable and create a more robust interface.

### Integration Events

Integration events are Domain Events like internal Domain Events.  However, they differ in that their primary purpose is less about indicating a particular state change within the Bounded Context, as it is in communicating the change to other Bounded Contexts, Applications and Systems.  This can influence how we model and shape our events.  Within a Bounded Context, we are always striving to catch the *why* of a state change. That is, we are not only interested in the fact that a user has been deactivated (*UserAccountDeactivated*) but for what reason (e.g. *SuspectedFraudAccountDetected*, *UserDeactivatedAccount*, etc.)  In another Bounded Context, where we are only interested in keeping a list of currently active accounts, we do not want to have to handle a number of specific events which result in the same effective state change (toggling user active / deactive).

When designing events, you should consider the purpose of the event, particularly with regard to the internal / external dichotomy.  Internal events tend to be thinner and more descriptive of the why.  Integration (external) events tend to be fatter and more broader in their description. The two types of events also have different trajectories in terms of their volatility and how often you can and should change them.  It is easier to evolve and adapt an internal event than an integration event.

### Metadata

An Event has a lot of common contextual information that is useful in the tracing and processing of the Event.  This metadata includes which Entity / Aggregate the event relates to, the User or System that caused the event, a Correlation id that allows the Event to be related to a Command or other action within the system, a UTC timestamp indicating when the Event occurred, the version of the Event and Aggregate, the Source (Application, Bounded Context, Tenant) and so on.  This combination of the Event and Contextual metadata is often referred to as the EventEnvelope.  The metaphor being that the metadata is the envelope and address and so on which the event is that actual contents of the letter.

Dolittle supports such Event Envelopes.  It is therefore redundant to include this contextual information on your Events.  It also draws attention away from the important aspects of the Event (the state change).
