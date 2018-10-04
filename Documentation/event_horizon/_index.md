---
title: Event Horizon
description: Learn about what an event horizon is and how it works in Dolittle
keywords: Runtime
author: einari
---

# The problem

When breaking up systems into smaller more digestible pieces using guidance such as [bounded contexts](../bounded_context),
you need to compose these systems back together. One of the goals when breaking things up is that the individual parts
become as autonomous as possible. This is to remove friction in development and deployment. Part of bringing it back together
often requires some communication between the running parts. With Dolittle being oriented around event sourcing and all state
transitions represented as events, its natural to let the events be the contract between bounded contexts and even between
systems.

Take for instance a domain like eCommerce with bounded contexts such as the warehouse and then the shop part.
When these systems are built individually but you want them to appear as part of the same product. You have to do a
composition of these making it look and feel as one. Part of being autonomous means that each bounded context has their own
instance of the resources it needs, such as databases and event store. They are in fact completely segregated and does not
couple themselves indirectly through common resources.

A composition of this would look like the following:

***TODO: Consistent look and feel - look at representing this differently***

![](./images/composition.png)

A lot of systems also has the complexity of being multi tenanted, meaning that the system has multiple customers using it.
Segregating resources such as database and event store for each tenant is a good strategy both from a security perspective
and a scale perspective.

The composition would then look like the following:

***TODO: Consistent look and feel - look at representing this differently***

![](./images/composition_multi_tenant.png)

Part of both of these compositions, sits the event horizon. A system that sits between the bounded contexts and knows how
to do communication between the different parts in a decoupled manner leaving each part to work autonomous.

One of the biggest challenges with communication between the systems is that the shape of the event might change over time.
This brings versioning to the table, which is probably the single biggest challenge in decoupled systems. Being able to deliver
on the promise of autonomy is very hard when you have systems relying on events from you.

## Event Horizon

At the heart of the Dolittle runtime sits the concept of an Event Horizon. With a vocabulary loosely based on [general relativity](https://en.wikipedia.org/wiki/General_relativity) and a tribute to the late professor [Stephen Hawking](https://en.wikipedia.org/wiki/Stephen_Hawkings) with a semi scientific approach linking the concepts to what they actually do in the software. You can find a fun video [here](https://www.youtube.com/watch?v=E8hzLM0JpYw) that explains event horizons in 60 seconds.

The event horizon is considered the last a bounded context will see of an event, once it moves past the horizon - the bounded context
will no longer see it. At the center of it sits singularities that will receive the event. To deal with the complexity of versioning
Dolittle has the concept of generations. Any artifact can shift shape during the lifetime of a system, these are represented as generations.
This is an essential concept for everything related to events. All events can be migrated between generations at any given point.
Coming out of an event store, the Dolittle runtime will deal with migrating the event based on migration strategies registered for the
artifact. The same principle is applied for events crossing over into other bounded contexts.

The different building blocks of the event horizon are as follows:

***TODO: Consistent look and feel - look at representing this differently***

![](./images/event_horizon.png)

Following are the definitions of each part of the vocabulary.

### [Event Horizon](https://en.wikipedia.org/wiki/Event_horizon)

The event horizon represents the final entry for committed events. At this point they can only be seen by other singularities.
In Dolittle, a singularity would then be a running node that is connected to this event horizon and receives a stream of particles.
The particles, representing committed events. By committed events, we mean events that has been persisted into an event store.

### [Singularity](https://en.wikipedia.org/wiki/Gravitational_singularity)

A singularity represents a single destination point for an event-particle. An event horizon can [spaghettify](https://en.wikipedia.org/wiki/Spaghettification) particles into multiple singularities.

### [Quantum Tunnel](https://en.wikipedia.org/wiki/Quantum_tunnelling)

Each singularity can connect to any event horizon, they establish a quantum tunnel for the purpose of passing particles through.

### [Wave Function](https://en.wikipedia.org/wiki/Wave_function)

Part of the process moving through a quantum tunnel means at times the [quantum state](https://en.wikipedia.org/wiki/Quantum_state) gets collapsed.
The state, being an event particle has the possibility to change between different versions of the the software.
This process is described sa [wave function collapse](https://en.wikipedia.org/wiki/Wave_function_collapse).
A undefined process in Dolittle, but seems interesting is the [wave function renormalization](https://en.wikipedia.org/wiki/Wave_function_renormalization).

### [Particle](https://en.wikipedia.org/wiki/Particle)

Particles are small objects, and in Dolittle there is an event particle. This is the thing that passes through the event
horizon into each singularity.

### [Barrier](https://en.wikipedia.org/wiki/Rectangular_potential_barrier)

For quantum tunnels to be opened from a singularity towards an event horizon, it has to penetrate the barrier.
This is the last line of defense for connecting - the barrier can refuse the opening of the tunnel.

### [Gravitational Lens](https://en.wikipedia.org/wiki/Gravitational_lens)

A gravitational lens is a distribution of matter between a distant light source and an observer, that is capable of bending the light from the source as the light travels towards the observer.
In order to observe black holes and its event horizons, one can do so through observing the gravitational lens.
Translated, this means the actual server that keeps the connection and observes (or in fact waits) for black holes with its quantum tunnels and singularities.

### [Geodesics](https://vrs.amsi.org.au/geodesic-incompleteness-spacetime/)

An observer travelling along a geodesic path may remain in motion forever, or the path may terminate after a finite amount of time. Paths that carry on indefinitely are called complete geodesics, and those that stop abruptly, incomplete geodesics.
This relates to how far in the offset in which a singularity, in our case a bounded context has reached when connected to an event horizon.
