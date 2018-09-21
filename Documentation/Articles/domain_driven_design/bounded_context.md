---
title: Bounded Context
description: Learn about Bounded Context
keywords: Runtime, Overview, Bounded Context
author: woksin
---
The key words “MUST”, “MUST NOT”, “REQUIRED”, “SHALL”, “SHALL NOT”, “SHOULD”, “SHOULD NOT”,
“RECOMMENDED”, “MAY”, and “OPTIONAL” in this document are to be interpreted as described in
[RFC 2119](https://tools.ietf.org/html/rfc2119).

## DDD Bounded Context Definition
A bounded context defines the boundaries within which a Domain Model and Ubiquitous Language are valid

## Intro
{{% notice info %}}
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
{{% /notice %}}

The core principle is to keep the different parts of your system apart and not take any dependency on any other contexts. We in Dolittle think that, when designing [Line of Business](https://en.wikipedia.org/wiki/Line_of_business) applications, the software design should strive for [loose coupling](https://en.wikipedia.org/wiki/Loose_coupling) and [high cohesion](https://en.wikipedia.org/wiki/Cohesion_(computer_science)). Understanding the domain and designing a solution that solves the business problem in a way that adheres to the customer's expectations is no trivial task. Given a large enough problem-domain (it shouldn't even have to be that big or complex), it should be practically impossible to grasp the whole problem and design the ideal system the first time around.  We must be able to adjust, or even completely replace components of the system, to provide new or adjusted functionality and to be able to keep up with the ever-changing requirements and poor translations of the system's intent and behaviour. Therefore it is with utmost importance that we today, and especially in the future, design software solutions with flexibility, scalability and maintainability in mind.

Domain-driven Design and bounded contexts are tools that can be used to deal with this. By exploring the problem-domain with domain-experts we'll discover a set of terms used by the domain-experts to explain and describe the moving parts of the domain. These terms form up a [ubiquitous language](https://martinfowler.com/bliki/UbiquitousLanguage.html) which in essence is just a set of words, agreed upon between the developers and the domain-experts, that forms up a conceptual description of the system. A bounded context is a confined area of the domain that usually just deals with a single business-capability, a sub-domain, in which a term in the ubiquitous language **MUST** only have a single meaning. For example, in E-commerce we could have defined a ubiquitous language where we have the term "product". But "product" can have different meaning depending on which business-capability of the system we're talking about. The business-capability that deals with "shopping carts" may only be interested in the price, availability, and the quantity of the item in the "shopping cart", while the sub-domain that deals with the stock management may only be interested in the product number, the number of items in stock and a location in storage. These two business-capabilities must be separated because the meaning of the term "product" varies depending on the context in which it it is used. We separate these business-capabilities into their own *Bounded Contexts*, achieving a full de-coupling of the term "product" between these. We can drastically change, or even remove, the meaning of the common term completely from either *Bounded Context* and the other wouldn't care at all. When applied correctly, this way of designing software will help us with maintaining a loosely coupled, highly cohesive system that captures the domain, is maintainable, flexible and robust wile it, at the same time, tackles the complexity of the problem.

{{% notice tip %}}
If you want to learn more about the concepts of *Bounded Context*, and also to learn more about Domain-Driven-Design in general, we highly suggest that you read [Domain-Driven-Design](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215) by Eric Evans. Martin Fowler is also a great resource, he has written about a lot of stuff that's relevant to those interested in building software, including [Bounded Contexts](https://martinfowler.com/bliki/BoundedContext.html). Check out his [website](https://martinfowler.com/)
{{% /notice %}}

## What it means technically

With this conceptual understanding we'll provide a more concrete description of an *Application* and a *Bounded Context*.
{{% notice note %}}
Independent of whatever perception you might have of bounded contexts, we in Dolittle have a very specific idea of what a *Bounded Context* is in our platform, what an application is and how they are built.
{{% /notice %}}
#### Application
The *Application* is the top-level building block of the software built upon the Dolittle platform. It bundles the subsystems that make up for the intended functionality of the application / software.
An *Application* is essentially a collection of Bounded Contexts, represented simply as a [GUID](https://en.wikipedia.org/wiki/Universally_unique_identifier), the behaviour of the *Application* is defined by the collective behaviour of its Bounded Contexts. It's what the tenants register as an application in their *Application Registry* and the thing that actually gets deployed.
A deployed application is observing change in each of the individual Bounded Contexts that form the application, a change in either of these Bounded Contexts will then implicitly change the version of the *Application*. Each [combination](https://en.wikipedia.org/wiki/Combination) of the set of the *Application's* Bounded Contexts corresponds to its own unique *Application* version. 

#### Bounded Context
A *Bounded Context* is a division of the *Application* with its own, well-defined, area of responsibility in the business domain with its own unique vocabulary that is commonly understood by the components of the *Bounded Context*, the domain-experts and the development team(s) working on the *Bounded Context*. Typically a *Bounded Context* would be structured as a single, self-contained, application, a project or a solution, with its own codebase, preferably separated from the other Bounded Contexts. It can only belong to a single *Application* and it will keep a reference, a [GUID](https://en.wikipedia.org/wiki/Universally_unique_identifier), which uniquely identifies the *Application* it belongs to.

Bounded Contexts **MUST NOT** have a dependency on another *Bounded Context* as this will result in high coupling. It'll not only defeat the purpose of modelling the application using bounded contexts in the first place but it will also set the stage for an exponential growth in complexity and technical debt when the solution scales and grows. If two Bounded Contexts shares a subset of the same terms, ambiguity of the terms would arise and it could, in the end, be the cause of unwanted increase in complexity and confusion. It would result in a loss of precision and focus, not only in the system itself, but also in the codebase and thus among the developers working on the *Bounded Context*. The *Bounded Context* **SHOULD** be defined in a manner which allows the components that make up the *Bounded Context* be as cohesive as possible. This makes perfect sense from a Domain-Driven-Design perspective, we want to define our bounded contexts to have responsibility for a small, confined, area of the domain. Its responsibilities **SHOULD NOT** touch multiple areas of the domain, this will inevitably lead to confusion, ambiguity and a loss of focus. A *Bounded Context* **SHOULD**, therefore, consist of components that belong closely together, working together towards a common goal, providing the functionality for a single business-capability. Building Bounded Contexts this way closely adheres to the core principles we in Dolittle have when it comes to building software, the [SOLID](https://en.wikipedia.org/wiki/SOLID) design principles and for preserving a strong [cohesion](https://codurance.com/software-creation/2016/03/03/cohesion-cornerstone-software-design/).

{{% notice info %}}
According to DDD, you can have some shared code between bounded contexts. They call this a shared kernel. It's permissible, but you'd have to think very, very carefully before doing it. Every sharing between *Bounded Contexts* is a coupling. Don't do it for trivial things (e.g. sharing concepts or value objects), just replicate them in the *Bounded Contexts* that needs them.
{{% /notice %}}

##### Topology; Modules and Features
The *Bounded Context* is an essential concept in DDD, but by now you should also have the impression that the *Bounded Context* is something very specific and concrete in the Dolittle platform. All Bounded Contexts has, for example, a very concrete structure. We want developers to think of Bounded Contexts as its own, stand alone, project. How you actually structure this is dependent on the programming language you're using, but in terms of C# we would suggest that you structure an *Application* this way:
![Application C# Structure](/overview/images/ApplicationStructure.png)

Here you can see how we in Dolittle would structure an *Application*. The *Application* would, for example, be a Github repository and it would typically have Bounded Contexts sitting inside its Source. Each *Bounded Context* is a folder with a solution (.sln) file and contains all the necessary domain areas; Concepts, Domain, Events, Read and an optional interaction layer called, in this case, Web.

Anyway, how you structure your actual *Application* is not that important, however, the internal structure of the *Bounded Context* and its domain areas is what's important. When you create a *Bounded Context* based on the Dolittle platform, and you have a reference to the Dolittle Build tool, what will happen when you compile the *Bounded Context* is that we'll look at the structure of your *Bounded Context* project and create a topology object that defines the topology of the *Bounded Context* and put it inside the bounded-context.json configuration file (configuration file is described later on). The topology will essentially be a recursive structure with features (or modules and features if modules is enabled, described later on). A feature, in terms of the Dolittle platform, is essentially just a way to group [*Artifacts*]**(LINK TO ARTIFACTS)**. *Features* are uniquely identified throughout the *Application* by a GUID (explained later). We group *Artifacts* together by *Feature*, this is to preserve a strong cohesion between the components that belongs together while at the same time we can also cross cut a lot of other concerns i.e. defining a user's access of the *Bounded Context* / *Application* based on which *Modules* / *Features* / *Artifacts* the user has authorization to read, write and/or execute. 

So for example if you've enabled the option to structure the topology with modules, the "Domain" area **SHOULD** look something like this if:
```
+-- Bounded Context 1
|   +-- Domain
|   |   Domain.csproj
|   |   +-- Module 1
|   |   +---- Feature 1
|   |       | Command.cs
|   |       | CommandInputValidator.cs
|   |       | CommandBusinessValidator.cs
|   |       | CommandHandler.cs
|   |       | SecurityDescriptor.cs
|   |       | CommandHandler.cs
|   |       | AggregateRoot.cs
|   |       | Service.cs
|   |       +-- Sub-Feature 1
|   |    |    | Command.cs
|   |    |    | CommandInputValidator.cs
|   |    |    | CommandBusinessValidator.cs
|   |    |    | CommandHandler.cs
|   |    |    | SecurityDescriptor.cs
|   |    |    | CommandHandler.cs
|   |    |    | AggregateRoot.cs
|   |    |    | Service.cs
|   |   +---- Feature 2
|   |       | Command.cs
|   |       | CommandInputValidator.cs
|   |       | CommandBusinessValidator.cs
|   |       | CommandHandler.cs
|   |       | SecurityDescriptor.cs
|   |       | CommandHandler.cs
|   |       | AggregateRoot.cs
|   |       | Service.cs
|   |   +-- Module 2
|   |   +---- Feature 1
...
```

##### Artifacts
A *Bounded Context* will eventually consist of a set of [*Artifacts*]**(LINK TO ARTIFACTS DOCS)**, they are what actually defines the behaviour and functionality of the *Bounded Context*. The *Artifacts* will be [Events]**(LINK TO EVENT DOC)**, [Commands](../../command/introduction), [Command Handlers](../../command/command_handler), [Aggregate Roots](../aggregate_root), [Queries](../../../read/query) and [Read Models](../../../read/read_model). You should read about the different types of *Artifacts* to gain an understanding of how they'll impact the *Application* / *Bounded Context*.
Each *Artifact* is an entity in the *Application*, uniquely identified throughout the *Application* in which the *Bounded Context* belongs. An *Artifact* belongs to a single Feature. 

## The configuration
{{% notice warning %}}
The configuration files are subject to change
{{% /notice %}}
When you create a new *Bounded Context* there are some configuration that needs to be done in order for the *Bounded Context* to work as intended. The configuration file is called bounded-context.json and it looks like this:
```json
{
  "application": "0d577eb8-a70b-4e38-aca8-f85b3166bdc2",
  "boundedContext": "f660966d-3a74-44e6-8268-a9aefbae6115",
  "boundedContextName": "Shop",
  "useModules": true,
  "generateProxies": true,
  "proxiesBasePath": "Features",
  "namespaceSegmentsToStrip": {
    "Events": [
      "Warehouse"
    ]
  },
  "topology": {
    "modules": [
      {
        "module": "8d5a724b-84eb-4085-a766-8d28e681743e",
        "name": "Carts",
        "features": [
          {
            "feature": "80f5e1a2-a2bc-4403-b7ec-8bd90920cf2a",
            "name": "Shopping",
            "subFeatures": []
          }
        ]
      },
      {
        "module": "c020195d-5675-4c17-9cc5-1a7539ce4680",
        "name": "SomeModule",
        "features": [
          {
            "feature": "728459c2-fab1-40c1-9ead-7122a1a890ea",
            "name": "SomeFeature",
            "subFeatures": []
          }
        ]
      },
      {
        "module": "9291da5e-a5ad-4dc7-9037-5c97fad04046",
        "name": "Catalog",
        "features": [
          {
            "feature": "05b89f06-19c3-4502-b349-873ef7761a21",
            "name": "Listing",
            "subFeatures": []
          }
        ]
      }
    ],
    "features": []
  }
}
```
* application - The GUID of the *Application* that this *Bounded Context* belongs to
* boundedContext - The GUID of the *Bounded Context*
* useModules - Whether or not the *Bounded Context* topology structure is Module-based or Feature-based. If it's Module-based then the Topology will consist of a list of *Modules* with a list of *Features*. If it's Feature-based it'll consist of just a list of *Features*.
* generateProxies - Whether or not we should generate, at the moment, ReadModel and Query proxy classes for the web interaction layer.
* proxiesBasePath - The base path relative to the path where the build is performed from where the proxies will be created.
* namespaceSegmentsToStrip - A dictionary of where the *Value* is a list of namespace-segments to strip from the namespace when creating the *Artifacts* when the first segment of the namespace is equal to the *Key*.
* topology: The topology structure object. This is automatically generated when you build the *Bounded Context*, given that you've already provided the other information described above.
