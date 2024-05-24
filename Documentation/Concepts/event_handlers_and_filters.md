---
title: Event Handlers & Filters
description: Overview of event handlers and filters
weight: 10
repository: https://github.com/dolittle/Runtime
---

In event-sourced systems it is usually not enough to just say that an [Event]({{< ref "events.md" >}}) occurred. You'd expect that something should happen as a result of that event occurring as well.

In the [Runtime]({{< ref "overview.md" >}}) we can register 2 different processors that can process events; [Event Handlers]({{< ref "#event-handlers">}}) and [Filters]({{< ref "#filters">}}).
They take in a [Stream]({{< ref "streams" >}}) of events as an input and does something to each individual event.

## Registration

In order to be able to deal with committed events, the heads needs to register their processors. The Runtime offers endpoints which initiates the registration of the different processors. Only registered processors will be ran. When the head disconnects from the Runtime all of the registered processors will be automatically unregistered and when it re-connects it will re-register them again. Processors that have been unregistered are idle in the Runtime until they are re-registered again.

## Scope

Each processor processes events within a single [scope]({{< ref "event_store#scope" >}}). If not specified, they process events from the default scope (eventlog). Events coming over the [Event Horizon]({{< ref "event_horizon" >}}) are saved to a scope defined by the event horizon [Subscription]({{< ref "event_horizon#subscription" >}}).


![Filter](/images/concepts/filter.png)

The filter is a powerful tool because it can create an entirely customized stream of events. It is up to the developer on how to filter the events, during filtering both the content and the metadata of the event is available for the filter to consider. If the filter creates a partitioned stream it also needs to include which partition the event belongs to.

However with great power comes great responsibility. The filters cannot be changed in a way so that it breaks the [rules of streams]({{< ref "streams#rules" >}}). If it does, the Runtime would notice it and return a failed registration response to the head that tried to register the filter.

### Public Filters

Since there are [two types of streams]({{< ref "streams.md#public-vs-private-streams" >}}) there are two kinds of filters; public and private. They function in the same way, except that private filters creates private streams and a public filter creates [public streams]({{< ref "streams#public-vs-private-streams" >}}). Only [public events]({{< ref "events#public-vs-private" >}}) can be filtered into a public stream.

## Event Handlers

The event handler is a combination of a filter and an event processor. It is identified by an `EventHandlerId` which will be both the id of both the filter and the event processor.

![Event Handler](/images/concepts/eventhandler.png)

The event handler's filter is filtering events based on the [`EventType`]({{< ref "events.md#event-type" >}}) that the event handler handles.

Event handlers can be either partitioned or unpartitioned. Partitioned event handlers uses, by default, the [`EventSourceId`]({{< ref "event_sourcing#event-source-id" >}}) of each event as the partition id.

### Replaying events

An event handler is meant to handle each events only once, however if you for some reason need to "replay" or "re-handle" all or some of the events for an event handler, you can use the [Dolittle CLI]({{< ref "docs/reference/cli" >}}) to initiate this while the microservice is running.

{{< alert title="Idempotence" color="warning" >}}
As creating a new event handler will handle all of its events, it's very important to take care of the handle methods side effects. For example, if the handler sends out emails those emails would be resent.
{{< /alert >}}

## Multi-tenancy

When registering processors they are registered for every tenant in the Runtime, resulting in every tenant having their own copy of the [Stream Processor]({{< ref "streams#multi-tenancy" >}}).
