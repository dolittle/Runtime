---
title: Read Model
description: About Read Models
keywords: read model, ddd, read, 
author: pavsaund, smithmx
---

# Read Model

A read model is a structure that is used to expose data to clients in an optimized way. This means structuring models that match the expected usage from clients.

To create a read model, mark your class with the marker interface `IReadModel`. The marker interface allows easy querying of *Read Models* by id from clients or allowing them to be exposed through [*Queries*](query).

```csharp
using Dolittle.Read;

public class Employee : IReadModel
{
    public EmployeeId Id { get {....} }
    public Name Name { get {....} }
    public SocialSecurityNumber SSN { get {....} }
    ...
}
```

## Structure
Read models are usually lightweight structures with a minimal amount of hierarchy to avoid complex queries and data structures required to populate the models.

## Single Responsibility Models
The intent of a read model is to provide a dat strcutre for a specific use case. When you need to provide a similar, but not same, data in another part of your application, the preferred approach in Dolittle is to create a new *Read Model* with it's own optimized storage.


## Concepts / Value Types
It's recommended to use [*Concepts* and *Value Objects*](../articles/domain_driven_design/concepts_and_value_objects) when defining properties in your read models.
