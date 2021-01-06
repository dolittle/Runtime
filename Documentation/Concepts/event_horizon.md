---
title: Event Horizon
description: Learn about what an event horizon is and how it works in Dolittle
weight: 10
---

Dolittle is a decentralized platform solution for for making highly scalable distributed systems.
A reliable ecosystem for microservices to thrive so that you can build complex applications with small, focused microservices that are loosely coupled and highly maintainable.
But having many microservices that are isolated and only confined to themselves does not provide much value.
There is clearly a need for microservices to talk with each other. The challenge is how should these independent microservices communicate with each other?
Of course we in Dolittle believe in the power of [events]({{< ref "events.md" >}}). We believe that information flows between microservices and systems through events.

## The problems

### Breaking it into smaller pieces

When breaking up systems into smaller more digestible pieces using guidance such as [bounded contexts](https://martinfowler.com/bliki/BoundedContext.html),
you need to compose these systems back together. One of the goals when breaking things up is that the individual parts
become as autonomous as possible. This is to remove friction in development and deployment. Part of bringing it back together
often requires some communication between the running parts. With Dolittle being oriented around event sourcing and all state
transitions represented as events, its natural to let the events be the contract between microservices and even between
systems.

Take for instance a domain like eCommerce with microservices such as the warehouse and then the shop part.
When these systems are built individually but you want them to appear as part of the same product. You have to do a
composition of these making it look and feel as one. Part of being autonomous means that each microservice has their own
instance of the resources it needs, such as databases and [event store]({{< ref "event_store.md" >}}). They are in fact completely segregated and does not
couple themselves indirectly through common resources.

### Multi-Tenancy

A lot of systems also has the complexity of being multi tenanted, meaning that the system has multiple customers using it.
Segregating resources such as database and event store for each tenant is a good strategy both from a security perspective
and a scale perspective.

### Api changes

One of the biggest challenges with communication between the systems is that the shape of the event might change over time.
This brings versioning to the table, which is probably the single biggest challenge in decoupled systems.
Being able to deliver on the promise of autonomy is very hard when you have systems relying on events from you.

## Event Horizon

At the heart of the Dolittle runtime sits the concept of an Event Horizon. With a vocabulary loosely based on [general relativity](https://en.wikipedia.org/wiki/General_relativity) and a tribute to the late professor [Stephen Hawking](https://en.wikipedia.org/wiki/Stephen_Hawkings) with a semi scientific approach linking the concepts to what they actually do in the software. You can find a fun video [here](https://www.youtube.com/watch?v=E8hzLM0JpYw) that explains event horizons in 60 seconds.

The event horizon is considered the last a microservice will see of an event, once it moves past the horizon - the microservice will no longer see it.
What we mean with that is that a microservice doesn't really know what happens with an event after it has gone past the event horizon, and nor does it care.
At the center of it sits singularities that will receive the event. The singularities makes sure that the received events are transmitted to the event store.

{{< alert title="About event migrations" color="warning" >}}
To deal with the complexity of versioning Dolittle has the concept of generations. Any artifact can shift shape during the lifetime of a system, these are represented as generations.
This is an essential concept for everything related to events. All events can be migrated between generations at any given point.
Coming out of an event store, the Dolittle runtime will deal with migrating the event based on migration strategies registered for the artifact. The same principle is applied for events crossing over the event horizon and into other microservices.
This is something we know that we need, but we've yet come up with a solution for this. As of now the generation number of events should not change.
{{< /alert >}}

From this point on we won't introduce any new terms in the general relativity lingo. We'll stick to what people are already familiar with: subscribers and producers.

### Subscriber

A subscriber is a [tenant]({{< ref "tenant.md" >}}) in a microservice that subscribes to a set of public events from a tenant in another microservice, producer.
It can subscribe to specific streams of events which will end up in the subscriber's event store, waiting for further processing.
More specifically, the subscriber subscribes to a specific [partition]({{< ref "streams.md#partitions" >}}) of a [public stream]({{< ref "streams.md#public-vs-private-streams" >}}) from another microservice. Meaning that every event that is put into that partition of that specific public stream in another microservice ultimately ends up in the subscriber's event store.
As with [event processors]({{< ref "event_handlers_and_filters.md" >}}) it is important that a subscription gets an event once, and once only and that it does not skip any events.
For that it uses the same mechanisms as the event processors (look at [stream processors]({{< ref "streams.md#stream-processor">}}) for details). Once the subscription has been successfully registered and the event horizon established the subscriber's microservice will start receiving events from from the producer.

### Producer

The producer is a tenant in a microservice that has subscribers subscribed to its public streams. When events are committed they can be committed as either public or private events. Only public events are eligible for being filtered into a public stream. It then has to explicitly configure a consent for a subscriber to subscribe to a partition in a public stream. The producer has to consent to a microservice subscribing to any of its public streams. When a subscription is received the producer's Runtime will check whether there exists a consent for that specific subscription and will only allow events to flow if that consent exists.
