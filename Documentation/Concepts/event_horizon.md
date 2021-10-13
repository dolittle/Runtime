---
title: Event Horizon
description: Learn about Event Horizon, Subscriptions, Consumers and Producers
weight: 15
---

At the heart of the Dolittle runtime sits the concept of Event Horizon. Event horizon is the mechanism for a microservice to give [Consent]({{< ref "#consent" >}}) for another microservice to [Subscribe]({{< ref "#subscription" >}}) to its [Public Stream]({{< ref "streams#public-vs-private-streams" >}}) and receive [Public Events]({{< ref "events#public-vs-private" >}}).

![Anatomy of an Event Horizon subscription](/images/concepts/eventhorizon.png)

## Producer

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

A subscription is setup by the [consumer]({{< ref "#consumer" >}}) to receive events from a [producer]({{< ref "#producer" >}}). Additionally the consumer has to add the producer to its [`microservices.json`]({{< ref "docs/reference/runtime/configuration#microservicesjson" >}}).

This is a simplified structure of a `Subscription` in the [consumer]({{< ref "#consumer" >}}).

```csharp
Subscription {
    // the producers microservice, tenant, public stream and partition
    MicroserviceId Guid
    TenantId Guid
    PublicStreamId Guid
    PartitionId string
    // the consumers scoped event log 
    ScopeId Guid
}
```

{{< alert title="Multiple subscriptions to same scope" color="warning" >}}
If multiple subscriptions route to the same [scoped event log]({{< ref "event_store#scope" >}}), the ordering of the events cannot be guaranteed. There is no way to know in which order the subscriber receives the events from multiple producers as they are all independent of each other.
{{< /alert >}}

## Event migration

{{% pageinfo color="warning" %}}
We're working on a solution for event migration strategies using [Generations]({{< ref "events#generations" >}}). As of now there is no mechanism for dealing with generations, so they are best left alone.
Extra caution should be paid to changing public events so as not to break other microservices consuming those events.
{{% /pageinfo %}}
