---
title: Bounded Context
description: Learn about Bounded Context
keywords: Overview, Bounded Context
author: Woksin
---
The key words “MUST”, “MUST NOT”, “REQUIRED”, “SHALL”, “SHALL NOT”, “SHOULD”, “SHOULD NOT”,
“RECOMMENDED”, “MAY”, and “OPTIONAL” in this document are to be interpreted as described in
[RFC 2119](https://tools.ietf.org/html/rfc2119).

## Intro
In a large system, you find that the system is not a single monolithic system, but rather a composition of smaller systems.
Rather than modelling these together as one, bounded contexts play an important role in helping you separate the different
subsystems and modelling these on their own. Putting it all together in one model tends to become hard to maintain over
time and often error-prone due to different requirements between the contexts that have yet to be properly defined.
We see that we often have some of the same data across a system and chose to model this only once - making the model
include more than what is needed for specific purposes. This leads to bringing in more data than is needed and becomes
a compromise. Take for instance the usage of [Object-relational mapping](https://en.wikipedia.org/wiki/Object-relational_mapping)
and a single model for the entire system approach. If you have a model with relationships and you, in reality, have different
requirements you end up having to do a compromise of how you fetch it. For instance, if one of your features displays all
the parts of the model including its children; it makes sense to eagerly fetch all of this to save roundtrips. While if
the same model is used in a place where only the top aggregate holds the information you need, you want to be able to
lazy load it so that only the root gets loaded and not its children. The simple solution to this is to model each of the
models for the different bounded contexts and use the power of the ORM to actually map to the database for the needs one
has.

The core principle is to keep the different parts of your system apart and not take any dependency on any other contexts. We in Dolittle think that, when designing [Line of Business](https://en.wikipedia.org/wiki/Line_of_business) applications, the software design should strive for [loose coupling](https://en.wikipedia.org/wiki/Loose_coupling) and [high cohesion](https://en.wikipedia.org/wiki/Cohesion_(computer_science)). Understanding the business domain and designing a solutions that solves the business problem in a way that adheres to the customer's expectations is no trivial task. Given a large enough problem-domain (it shouldn't even have to be that big or complex), it should be practically impossible to grasp the whole problem and design the ideal system the first time around.  We must be able to adjust, or even completely replace components of the system, to provide new or adjusted functionality and to be able to keep up with the ever-changing requirements and poor translations of the system's intent and behaviour. Domain-Driven Design and bounded contexts are tools that can be used to deal with this. By exploring the problem-domain with domain-experts we'll discover a set of terms used by the domain-experts to explain the domain, these terms form up a [ubiquitous language](https://martinfowler.com/bliki/UbiquitousLanguage.html), a Bounded Context is a confined area of the domain in which a term in the ubiquitous language MUST only have a single meaning. For example, in E-commerce we talk about the term "product". In the domain of E-commerce, the term "product" can have different meaning depending on which business-capability of the system we're talking about. The business-capability that deals with "shopping carts" may only be interested in the price, availability, and the quantity of the item in the "shopping cart", while the sub-domain that deals with the stock management may only be interested in the product number, the number of items in stock and a location in storage. These two business-capabilities must be separated because the meaning behind the term "product" varies depending on the context in which it it is used. We separate these business-capabilities into their own Bounded Contexts, achieving a full de-coupling of the term "product" between these. We can drastically change, or even remove, the meaning of the common term completely from either Bounded Context and the other wouldn't care at all. When applied correctly, this way of designing software will help us with maintaining a loosely coupled, highly cohesive system that captures the domain, is maintainable, flexible and robust wile it, at the same time, tackles the complexity of the problem.


If you want to learn more about the concepts of Bounded Context, and also to learn more about Domain-Driven-Design in general, we highly suggest that you read [Domain-Driven-Design](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215) by Eric Evans.


## What it means technically

With this conceptual understanding we'll provide a more concrete description of an Application and a Bounded Context. 
Independent of whatever perception you might have of bounded contexts, we in Dolittle have a very specific idea of what a Bounded Context is in our platform, what an application is and how they are built.

#### Application
The Application is the top-level building block of the software built upon the Dolittle platform. It bundles the subsystems that makes up for the intended functionality of the application / software.
An Application is essentially a collection of Bounded Contexts, represented simply as a [GUID](https://en.wikipedia.org/wiki/Universally_unique_identifier), the behaviour of the Application is defined by the collective behaviour of its Bounded Contexts. It's what the tenants register as an application in their Application Registry **(PUT LINK TO STUDIO STUFF HERE?)** and the thing that actually gets deployed.
The application is observing change in each of the individual Bounded Contexts that form the application. A change in either of these Bounded Contexts will then implicitly change the version of the Application. Each [combination](https://en.wikipedia.org/wiki/Combination) of the set of the Application's Bounded Contexts corresponds to its own unique Application version. 

#### Bounded Context
A Bounded Context is a small division of the application with its own, well-defined, area of responsibility in the business domain with its own unique vocabulary that is commonly understood by the components of the Bounded Context, the domain-experts and the development team(s) working on the Bounded Context. It can only belong to a single Application and it will keep a reference to the its Application via a [GUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) which fully represents the Application. 
Bounded Contexts MUST NOT have a dependency on another Bounded Context, this will result in high coupling and thus defeats the purpose of modelling the application using bounded contexts in the first place. If two Bounded Contexts shared a subset of the same terms, ambiguity of the terms would arise and it could, in the end, be the cause of unwanted increase in complexity. It SHOULD consist of components that belong together, working together towards a common goals, providing the functionality for a single business-capability. Building Bounded Contexts this way closely adheres to the core principles we in Dolittle have when it comes to building software, the [SOLID](https://en.wikipedia.org/wiki/SOLID) design principles and for preserving high cohesion. 

* The single source of truth - The only producer of a type of event **(INSERT LINK TO EVENTS HERE)**. 
* No Bounded Contexts should produce the same events .

* TODO: Artifacts

## The configuration

* Explain .dolittle folder (bounded-context.json, artifacts.json)
* C# (.NET) application structure, application domain areas, of Bounded Contexts
