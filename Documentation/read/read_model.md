---
title: Read Model
description: About Read Models
keywords: read model, ddd, read, 
author: pavsaund, tomasekeli
---

# Principle

A read model is a structure that is used to expose data to clients in an optimized way. The guiding principle is to have the *Read Model* pre-generated. Any query should be a simple lookup. As a consequence the *User* does not wait for work by the system when they need data.

## Structure
To create a *Read Model* mark your class with the marker interface `IReadModel`. *Read Models* exist only to be exposed through [*Queries*]({{< relref query >}}), just like the *Queries* exist only to expose *Read.

```csharp
using Dolittle.ReadModels;

public class ShoppingCartPreview : IReadModel
{
    public CartId Id { get {...} }
    public Quantity ItemsInCart { get {...} }
    public Amount Total { get {...} }
}
```

Read models should be lightweight structures with minimal complexity. They do not need to reflect the structure of any underlying Data Model. A *Read Model* should not be regarded as an *Entity*, but rather a document tailored for some specific use-case.

{{% notice tip %}}
Avoid complex queries and data structures required to populate the models. We recommend pre-aggregating *Read Models* where possible.
{{% /notice %}}
 
The `ShoppingCartPreview` *Read Model* above could very well be backed by some deeper model of Carts, Items, Products, Discounts, etc; but this is not surfaced to the *Read Model* as it is not needed by the *View* this *Read Model* is created for. The *Read Model* could be created whenever an event from the write-side occurs so no logic or aggregation is needed when querying.  

### Concepts / Value Types
The *Read Models* should speak in the language of the domain (*Ubiquitous Langauge*). You should therefor use [*Concepts* and *Value Objects*]({{< relref concepts_and_value_objects >}}) when defining properties in your *Read Models*. *Read Models* with primitive properties can be considered a code smell.

## Single Responsibility Models
The intent of a *Read Model* is to provide a data structure for a specific use case. When you need to provide a similar, but not same, data in another part of your application, the preferred approach in Dolittle is to create a new *Read Model* with it's own optimized storage. In fact, even if the needs of different use case are covered by an existing *Read Model* you should not re-use the *Read Model*. Doing so would introduce coupling between the use-cases in unintended and surprising ways.

{{% notice tip %}}
Avoid being *DRY* with *Read Models*. When making *Read Models you should prefer duplication over unintended coupling. 
{{% /notice %}}


## Read Models on existing data sources

In the case of an event-sourced system there will be no *Entity* -model, instead events are aggregated to specific *Read Models* as they occur. But, sometimes you have to work with an existing data source, and that will often be in the form of a database with current-state. It is recommended to flatten deep data-models where it will make the specific use of the *Read Model* simpler. The goal of a *Read Model* is usually to support some view of the data, and they should tailor it to the specific needs of that view. This can be done by using specific technology based on the current data source (e.g. materialised views in SQL databases), or by pre-aggregating and storing the *Read Models* for quick querying.

If it is not possible to pre-aggregate or make optimised data-structures the *Query* must do the hard work of retrieving and mapping the existing *Entities* into the *Read Model*. This may come at a high performance cost, as the system is doing a lot of work while the user is waiting. 

