---
title: Overview
description: Get a high-level outline of Dolittle and it's components
weight: 1
repository: https://github.com/dolittle/Runtime
---

Dolittle is a decentralized, distributed, event-driven microservice library built to harness the power of events.

## Components
The Dolittle stack is composed of the SDKs, the Runtime, and the [Event Store]({{< ref "event-store" >}}).

The user code (also called _Head_) is written using the SDKs, which connect to the Runtime.

The runtime is the backend of our system and manages connections from the SDKs and other runtimes to its Event Store. It's called the runtime as it's what runs and powers the SDK's to do their job.

The Event Store is the underlying database where the events are stored.

[Events]({{< ref "events" >}}) are "facts that have happened" in your system and they form the state/_truth_ of the system.

When runtimes subscribe to events from other runtimes, these events are transferred over what we call the [Event Horizon]({{< ref "event-horizon" >}}).

## Event Driven
Dolittle uses a style of Event-Driven Architecture called [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html), which means to _"capture all changes to an applications state as a sequence of events"_, these events then form the _"truth"_ of the system. Events **cannot be changed or deleted** as they represent things that have happened.

With event sourcing your applications state is no longer stored as a snapshot of your current state but rather as a whole history of all state changing events. These events can then be replayed to recreate the state whenever needed, eg. replay them to a test environment to see how it would behave. The system can also produce the state it had at any point in time.

Event sourcing allows for high scalability thanks to being a very loosely coupled system, eg. a stream of events can keep a set of in-memory databases updated instead of having to query a master database.

The history of events also forms an audit log to help with debugging and auditing.

### Distributed & Decentralized
Dolittle applications are built from microservices that communicate with each other using events. These microservices can scale and fail independently as there is no centralized message bus like in [Kafka](https://kafka.apache.org/). The runtimes and event stores are independent of other parts of the system.

## Microservice
A _microservice_ consists of one or many heads talking to one or many runtimes. The core idea is that a microservice is an independently scalable unit of deployment that can be reused in other parts of the software however you like.

This diagram shows the anatomy of a microservice with one head.

![Example anatomy of a Dolittle microservice](/images/concepts/anatomy.png)

{{< alert title="Read Cache" color="info" >}}
The _Read Cache_ in these pictures is not part of Dolittle. Different situations call for different databases depending on the sort of load and data to be stored.
{{< /alert >}}

### Multi-tenancy
Multi-tenancy means that a single instance of the software and its supporting infrastructure serves multiple customers. Dolittle supports multi-tenancy by separating the event stores for each tenant so that each tenant can only its own data.

This diagram shows a microservice with 2 tenants, each of them with their own resources.

![Example of multi-tenant microservice](/images/concepts/multitenant.png)

### What Dolittle isn't
Dolittle is not a traditional backend library nor an event driven message bus like [Kafka](https://kafka.apache.org/). Dolittle uses _Event Sourcing_, which means that the state of the system is built from an immutable event store that has all the events ever produced by the application.

Dolittle does not provide a read cache. Different situations call for different databases depending on the sort of load and data to be stored. The event store only defines how the events are written in the system, it doesn't define how things are read of interpreted.

## Technology
- [Runtime repository](https://github.com/dolittle/runtime)
- [C# SDK repository](https://github.com/dolittle/dotnet.sdk)
- [JavaScript SDK repository](https://github.com/dolittle/javascript.sdk)
- The connection between the runtime and the SDKs is managed through [gRPC](https://grpc.io/) calls, defined in our [Contracts repository](https://github.com/dolittle/contracts)

The Event Store is implemented with [MongoDB](https://www.mongodb.org/).

## What's next
- Read about [Events]({{< ref "events" >}})
- Ready to [Get Started]({{< ref "tutorials" >}})?
