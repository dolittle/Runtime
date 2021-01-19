---
title: Event Handlers & Filters
description: Overview of event handlers and filters
weight: 10
repository: https://github.com/dolittle/Runtime
---

In event-driven systems it is usually not enough to just say that an [Event]({{< ref "events.md" >}}) occurred. You'd expect that something should happen as a result of that event occurring as well.

In the [Runtime]({{< ref "overview.md" >}}) we can register 2 different processors that can process events; [Event Handlers]({{< ref "#event-handlers">}}) and [Filters]({{< ref "#filters">}}).
They take in a [Stream]({{< ref "streams" >}}) of events as an input and does something to each individual event.

Each of these processors is a combination of one or more [Stream Processors]({{< ref "streams#stream-processor" >}}) and [Event Processor]({{< ref "streams#event-processor" >}}).
What it does to the event is dependent on what kind of processor it is. We'll talk more about different processors later in this section.

## Registration

In order to be able to deal with committed events, the heads needs to register their processors. The Runtime offers endpoints which initiates the registration of the different processors. Only registered processors will be ran. When the head disconnects from the Runtime all of the registered processors will be automatically unregistered and when it re-connects it will re-register them again. Processors that have been unregistered are idle in the Runtime until they are re-registered again.

## Scope

Each processor processes events within a single [scope]({{< ref "event_store#scope" >}}). If not specified, they process events from the default scope. Events coming over the [Event Horizon]({{< ref "event_horizon" >}}) are saved to a scope defined by the event horizon [Subscription]({{< ref "event_horizon#subscription" >}}).

## Filters

The filter is a processor that creates a new stream of events from the [event log]({{< ref "event_store#event-log" >}}). It is identified by a `FilterId` and it can create either a partitioned or unpartitioned stream. The processing in the filter itself is however not partitioned since it can only operate on the event log stream which is an unpartitioned stream.

![Filter](/images/concepts/filter.png)

The filter is a powerful tool because it can create an entirely customized stream of events. It is up to the developer on how to filter the events, during filtering both the content and the metadata of the event is available for the filter to consider. If the filter creates a partitioned stream it also needs to include which partition the event belongs to.

However with great power comes great responsibility. The filters cannot be changed in a way so that it breaks the [rules of streams]({{< ref "streams#rules" >}}). If it does, the Runtime would notice it and return a failed registration response to the head that tried to register the filter.

### Public Filters

Since there are [two types of streams]({{< ref "streams.md#public-vs-private-streams" >}}) there are two kinds of filters; public and private. They function in the same way, except that private filters creates private streams and a public filter creates [public streams]({{< ref "streams#public-vs-private-streams" >}}). Only [public events]({{< ref "events#public-vs-private" >}}) can be filtered into a public stream.

## Event Handlers

The event handler is a combination of a filter and an event processor. It is identified by an `EventHandlerId` which will be both the id of both the filter and the event processor.

![Event Handler](/images/concepts/eventhandler.png)

The event handler's filter is filtering events based on the [`EventType`]({{< ref "events.md#event-type" >}}) that the event handler handles.

Event handlers can be either partitioned or unpartitioned. Partitioned event handlers uses, by default, the [`EventSourceId`]({{< ref "event_sourcing.md#event-source-id" >}}) of each event as the partition id. The filter follows the same rules [for streams]({{< ref "streams#rules" >}}) as other filters.


{{< alert title="Changing event handlers" color="warning" >}}
The event handler registration fails if your event handler suddenly stops handling an event type that it has already handled, or starts handling a new event type that has already occurred in the event log.
{{< /alert >}}

## Multi-tenancy

When registering processors they are registered for every tenant in the Runtime, resulting in every tenant having their own copy of the [Stream Processor]({{< ref "streams#multi-tenancy" >}}).
