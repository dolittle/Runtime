---
title: Overview
description: Get a high-level outline of Dolittle and it's components
weight: 1
repository: https://github.com/dolittle/Runtime
---

Dolittle is a decentralized, distributed, event-driven microservice platform built to harness the power of events. It's a reliable ecosystem for microservices to thrive so that you can build complex applications with small, focused microservices that are loosely coupled, event driven and highly maintainable.

## Components
<!-- The Dolittle stack is composed of the SDKs, the Runtime, and the [Event Store]({{< ref "event_store" >}}). -->

- [**Events**]({{< ref "events" >}}) are _"facts that have happened"_ in your system and they form the _truth_ of the system.
- [**Event Handlers & Filter**]({{< ref "event_handlers_and_filters" >}}) and [**Projections**]({{< ref "projections" >}}) process events.
- The **Runtime** is the core of all Dolittle applications and manages connections from the SDKs and other Runtimes to its [Event Store]({{< ref "event_store" >}}). The Runtime is packaged as a [Docker image](https://hub.docker.com/r/dolittle/runtime)
- The [**Event Store**]({{< ref "event_store" >}}) is the underlying database where the events are stored.
- The **Head** is the user code that uses the SDKs, which connect to the Runtime in the same way as a client (SDK) connects to a server (runtime).
- A [**Microservice**]({{< ref "#microservice" >}}) is one or more Heads talking to a Runtime.
- Microservices can [produce]({{< ref "event_horizon#producer" >}}) and [consume]({{< ref "event_horizon#consumer" >}}) events between each other over the [**Event Horizon**]({{< ref "event_horizon" >}}).

## Event-Driven
Dolittle uses a style of Event-Driven Architecture called [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html), which means to _"capture all changes to an applications state as a sequence of events"_, these events then form the _"truth"_ of the system. Events **cannot be changed or deleted** as they represent things that have happened.

With event sourcing your applications state is no longer stored as a snapshot of your current state but rather as a whole history of all state-changing events. These events can then be replayed to recreate the state whenever needed, eg. replay them to a test environment to see how it would behave. The system can also produce the state it had at any point in time.

Event sourcing allows for high scalability thanks to being a very loosely coupled system, eg. a stream of events can keep a set of in-memory databases updated instead of having to query a master database.

The history of events also forms an audit log to help with debugging and auditing.

### Distributed & Decentralized
Dolittle applications are built from microservices that communicate with each other using events. These microservices can scale and fail independently as there is no centralized message bus like in [Kafka](https://kafka.apache.org/). The Runtimes and event stores are independent of other parts of the system.

## Microservice
A _microservice_ consists of one or many heads talking to one Runtime. Each microservice is autonomous and has its own resources and [event store]({{< ref "event_store" >}}).

The core idea is that a microservice is an independently scalable unit of deployment that can be reused in other parts of the software however you like. You could compose it back in one application running inside a single process, or you could spread it across a cluster. It really is a deployment choice once the software is giving you this freedom. 

This diagram shows the anatomy of a microservice with one head.

![Example anatomy of a Dolittle microservice](/images/concepts/anatomy.png)

{{< alert title="Read Cache" color="info" >}}
The _Read Cache_ in these pictures is not part of Dolittle. Different [projections]({{< ref "event_sourcing#projections" >}}) call for different solutions depending on the sort of load and data to be stored.
{{< /alert >}}


### Multi-tenancy
Since computing is the most expensive resource, the Dolittle Runtime and SDK's has been built from the ground up with multi-tenancy in mind. Multi-tenancy means that a single instance of the software and its supporting infrastructure serves multiple customers, making optimal use of resources. Dolittle supports multi-tenancy by separating the event stores for each tenant so that each tenant only has access to its own data.

This diagram shows a microservice with 2 tenants, each of them with their own resources.

![Example of multi-tenant microservice](/images/concepts/multitenant.png)

## What Dolittle isn't
Dolittle is not a traditional backend library nor an event driven message bus like [Kafka](https://kafka.apache.org/). Dolittle uses [Event Sourcing]({{< ref "event_sourcing" >}}), which means that the state of the system is built from an append-only [Event Store]({{< ref "event_store" >}}) that has all the events ever produced by the application.

Dolittle does not provide a solution for [read models/cache]({{< ref "event_sourcing#projections" >}}). Different situations call for different databases depending on the sort of load and data to be stored. The event store only defines how the events are written in the system, it doesn't define how things are read or interpreted.

Dolittle isn't a CQRS framework, but [it used to be](https://github.com/dolittle/Bifrost).

## Technology
- [Runtime repository](https://github.com/dolittle/runtime)
- [C# SDK repository](https://github.com/dolittle/dotnet.sdk)
- [JavaScript SDK repository](https://github.com/dolittle/javascript.sdk)
- The connection between the runtime and the SDKs is managed through [gRPC](https://grpc.io/) calls, defined in our [Contracts repository](https://github.com/dolittle/contracts)

The Event Store is implemented with [MongoDB](https://www.mongodb.org/).

## What's next
- Read about [Events]({{< ref "events" >}})
- Ready to [Get Started]({{< ref "tutorials" >}})?
