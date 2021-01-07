---
title: Event Horizon
description: Learn about Event Horizon, Subscriptions, Consumers and Producers
weight: 10
---

At the heart of the Dolittle runtime sits the concept of Event Horizon. Event horizon is the mechanism for a microservice to give [Consent]({{< ref "#consent" >}}) for another microservice to [Subscribe]({{< ref "#subscription" >}}) to its [Public Stream]({{< ref "streams#public-vs-private-streams" >}}) and receive [Public Events]({{< ref "events#public-vs-private" >}}).
<!-- diagram here showing the subscriver and consumer and the streams, partitions, scoped event log etc -->
<!-- There could also be an example consent that shows how the setup in the example diagram would look like. and a link to the reference config page -->
![Anatomy of an Event Horizon subscription](/images/concepts/eventhorizon.png)

## Producer
<!-- also lets have example of microservices.json maybe? or maybe its too much. i sense a tutorial a brewing. -->
The producer is a [Tenant]({{< ref "tenants" >}}) in a [Microservice]({{< ref "overview#microservice" >}}) that has one or more [public streams]({{< ref "streams#public-vs-private-streams">}}) that [Consumer]({{< ref "#consumer" >}}) can subscribe to.
Only [public events]({{< ref "events#public-vs-private" >}}) are eligible for being filtered into a public stream.

Once an event moves past the event horizon, the producer will no longer see it. The producer doesn't know or care, what happens with an event after it has gone past the event horizon.

### Consent

The producer has to give consent for a consumer to subscribe to a [Partition]({{< ref "streams#partitions" >}}) in the producers public stream. Consents are defined in [`event-horizon-consents.json`]({{< ref "docs/reference/runtime/configuration#event-horizon-consentsjson" >}}).

## Consumer

A consumer is a [tenant]({{< ref "tenants" >}}) that subscribes to a [partition]({{< ref "streams#partitions" >}}) in one of the [Producer's]({{< ref "#producer" >}}) [public streams]({{< ref "streams#public-vs-private-streams" >}}). The events coming from the producer will be stored into a [Scoped Event Log]({{< ref "event_store#scope" >}}) in the consumer's [event store]({{< ref "event_store" >}}). This way even if the producer would get removed or deprecated, the produced events are still saved in the consumer.
To process events from a scoped event log you need [scoped event handlers & filters]({{< ref "event_handlers_and_filters#scope" >}}).

The consumer sets up the subscription and will keep asking the producer for events. The producers [Runtime]({{< ref "overview#components" >}}) will check whether it has a [consent]({{< ref "#constent" >}}) for that specific subscription and will only allow events to flow if that consent exists. If the producer goes offline or doesn't consent, the consumer will keep retrying.

### Subscription

A subscription is setup by the [consumer]({{< ref "#consumer" >}}) to receive events from a [producer]({{< ref "producer" >}}). Additionally the consumer has to add the producer to its [`microservices.json`]({{< ref "docs/reference/runtime/configuration#microservicesjson" >}}).

This is a simplified structure of a `Subscription` in the [consumer]({{< ref "#consumer" >}}).

```csharp
Subscription {
    // the producers microservice, tenant, public stream and partition
    MicroserviceId Guid
    TenantId Guid
    PublicStreamId Guid
    PartitionId Guid
    // the consumers scoped event log 
    ScopeId Guid
}
```

{{< alert title="Multiple subscriptions to same scope" color="warning" >}}
If multiple subscriptions route to the same [scoped event log]({{< ref "event_log#scope" >}}), the ordering of the events cannot be guaranteed. There is no way to know in which order the subscriber receives the events from multiple producers as they are all independent of each other.
{{< /alert >}}

## Event migration

{{% pageinfo color="warning" %}}
We're working on a solution for event migration strategies using [Generations]({{< ref "events#generations" >}}). As of now there is no mechanism for dealing with generations, so they are best left alone.
Extra caution should be paid to changing public events so as not to break other microservices consuming those events.
{{% /pageinfo %}}


<!-- 

Dolittle is a decentralized platform solution for making highly scalable distributed systems.
A reliable ecosystem for microservices to thrive so that you can build complex applications with small, focused microservices that are loosely coupled and highly maintainable.
But having many microservices that are isolated and only confined to themselves does not provide much value.
There is clearly a need for microservices to talk with each other. The challenge is how should these independent microservices communicate with each other?
Of course we in Dolittle believe in the power of [events]({{< ref "events.md" >}}). We believe that information flows between microservices and systems through events.

talk about the resubscribing so taht if a producer goes down it can come abck up later


## The problems

### Breaking it into smaller pieces

do we want to mention bouned context? guess its a fine pattern and all
When breaking up systems into smaller more digestible pieces using guidance such as [bounded contexts](https://martinfowler.com/bliki/BoundedContext.html),
you need to compose these systems back together. One of the goals when breaking things up is that the individual parts
become as autonomous as possible. This is to remove friction in development and deployment. Part of bringing it back together
often requires some communication between the running parts. With Dolittle being oriented around event sourcing and all state
transitions represented as events, its natural to let the events be the contract between microservices and even between
systems.

we could make this in the kitchen domain for consistency points
segregation could be said shorter
Take for instance a domain like eCommerce with microservices such as the warehouse and then the shop part.
When these systems are built individually but you want them to appear as part of the same product. You have to do a
composition of these making it look and feel as one. Part of being autonomous means that each microservice has their own
instance of the resources it needs, such as databases and [event store]({{< ref "event_store.md" >}}). They are in fact completely segregated and does not
couple themselves indirectly through common resources.

### Multi-Tenancy
we already have multi tenancy docs, no need to mention this here
A lot of systems also has the complexity of being multi-tenanted, meaning that the system has multiple customers using it.
Segregating resources such as database and event store for each tenant is a good strategy both from a security perspective
and a scale perspective.

### Api changes
I think talking bout changes could be its own section
One of the biggest challenges with communication between the systems is that the shape of the event might change over time.
This brings versioning to the table, which is probably the single biggest challenge in decoupled systems.
Being able to deliver on the promise of autonomy is very hard when you have systems relying on events from you.
-->
