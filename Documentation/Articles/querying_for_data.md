---
title: Querying for Data
description: About Reading data
keywords: read model, query, ddd, read, cqrs,  
author: pavsaund
---


# Querying for Data

## Introduction
The query or read side of the CQRS pattern is all about exposing the current state of a system in a format that is ready to be consumed by interaction layers (ie: web-based user interface). The state should be exposed through models that are optimized for querying, and is usually updated by changes made to a system through events.

{{% notice note %}}
Dolittle exposes read data through two different mechanisms:
 
 - [Read Models](https://github.com/dolittle/Runtime/tree/master/Documentation/read/read_models.md)
 - [Queries](https://github.com/dolittle/Runtime/tree/master/Documentation/read/queries.md)
{{% /notice %}}

### Updating Read Data
[Event Processors](LINK TO EVENT PROCESSORS) are responsible for consuming events in the system and updating read-models.

### Storing Data 
#### Optimize for querying
Unlike traditional relational data models that are spread across tables, read data should be optimized as much as possible to the clients consuming that data. This means creating models that can be consumed directly without additional queries. One of the of the benefits is that reading data becomes lightweight, fast and allows for good user experiences.

Duplicating data in 

{{% notice tip %}}
Think of data on the query side as your applications cache. It get's updated whenever there's a change to the system, and is fast by default.
{{% /notice %}}

##### Example of optimizing for querying:
In an e-commerce solution, you get a requirement to show a small-cart summary with Number of Items and total price. This is a challenge since it will get the current ShoppingCartModel each time the user hits any page of the site, and it adds a visible delay to the user.

A possible workaround could be to introduce a second cart model that only cared about the small-cart information and had pre-calculated toals.

Full ShoppingCartModel:
```
ShoppingCartModel
    |-CartId
    |-CartItems[]
        |-ProductName
        |-Price
        |-TaxRate
        |-Quantity
        |-Dimensions
        |-...
    |-AdditionalServices[]
        |-ServiceType
        |-Price
        |-Quantity
        |-...
    |-GiftCards[]
        |-...
    |-VoucherCodes[]
    |-Totals
        |-VatRate
        |-Net
        |-Gross
```

Focused SmallCartModel
```
SmallCartModel
    |-CartId
    |-ItemsCount
    |-Totals
        |-VatRate
        |-Net
        |-Gross
```

Now with the new model, queries can be made for the SmallCartModel on every page load, instead of the larger ShoppingCartModel.



#### Adapt storage
Another benefit is that read data can be persisted in the most suited format and not limited to any other storage mechanisms. Examples could be: Document Databases, JSON-files, XML-files, Relational DB, In-memory, Excel / PDF files etc. 

{{% notice tip %}}
Use the storage mechanism that most suits your use cases for reading data like generating a json file persisted to a CDN ready to be consumed by a web-frontend.
{{% /notice %}}


## Eventual consistency / Asynchronous


