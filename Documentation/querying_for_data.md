---
title: Querying for Data
description: About Reading data
keywords: read model, query, ddd, read, cqrs,  
author: pavsaund
aliases:
    - /runtime/runtime/querying_for_data
---


# Querying your System

## Introduction
The query or read side of the CQRS pattern is all about exposing the current state of your system in a format that is optimized for particular requirements. Since there are competing and conflicting requirements, no single data storage technology or data format will be optimal for all scenarios. It is therefore to be expected that in a sufficiently complex system there will be polyglot persistence and querying. A typical example, where the main requirement is to display information to an end user, might involve persistence to a document database or relational database. Alternatively, querying might involve populating a search index or nodes in a graph database.

Since state is updated through event subscriptions, we can store the state in multiple forms and for multiple purposes, with a query optimized for a specific purpose.

{{% notice note %}}
To help with common scenarios such as presentation of data to end users, decision supports, etc., Dolittle provides two helpers for retreiving data:
 
 - [Read Models](https://github.com/dolittle/Runtime/tree/master/Documentation/read/read_model.md)
 - [Queries](https://github.com/dolittle/Runtime/tree/master/Documentation/read/query.md)
{{% /notice %}}

### Storing Data 
#### Optimize for querying
Unlike traditional relational data models that are spread across tables, read data should be optimized as much as possible to the clients consuming that data. This means creating models that can be consumed directly without additional queries. One of the of the benefits is that reading data becomes lightweight, fast and allows for good user experiences.

Duplicating data in several places to optimise for reads is considered a good thing when using Dolittle. It is the responsibility of the Command-side to keep these optimised structures up-to-date and correct. The Read-side merely gives the data it has access to with minimal logic and fuss. This might seem foreign and weird, but it simplifies both the read and write sides a lot, and lets systems be responsive and performant on the read-side independently of the write-side.

{{% notice tip %}}
Write specific read-models for specific needs.
{{% /notice %}}

##### Example of optimizing for querying:
In an e-commerce solution, you get a requirement to show a small-cart summary with Number of Items and Total price on the header over every page. You may be tempted to reuse the full ShoppingCartModel that is used on the cart page, instead with Dolittle you'd look at how to crate an optimized model for that specific view need.

A possible workaround could be to introduce a second cart model that's only concern was the small-cart information and had pre-calculated toals. The benefit is the seperation of concers betwwen the two models, and an optimized model containing only whats needed for the view.

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
Another benefit is that read data can be persisted in the most suited format and not limited to any other storage mechanisms. Examples could be: Document Databases, JSON-files, XML-files, Relational DB, In-memory, Excel / PDF files etc. Or even fully rendered html documents (perhaps with JSON models inlined when interaction is needed) - pre-computed and available just in case the page is ever needed.

{{% notice tip %}}
Use the storage mechanism that most suits your use cases for reading data like generating a json file persisted to a CDN ready to be consumed by a web-frontend.
{{% /notice %}}


## Eventual consistency / Asynchronous updates


