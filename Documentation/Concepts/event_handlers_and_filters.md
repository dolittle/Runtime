---
title: Event Handlers & Filters
description: Get a deeper understanding of how Runtime processes events
weight: 10
repository: https://github.com/dolittle/Runtime
---

In event-driven systems it is usually not enough to just say that an [event]({{< ref "events.md" >}} "Events") occurred. Usually you'd expect that something should happen as a result of that event occurring as well.

In the [Runtime]({{< ref "overview.md" >}}) we have 3 constructs that can process events; [Event Handlers]({{< ref "#event-handlers">}}), [Filters]({{< ref "#filters">}}) and [Event Processors]({{< ref "#event-processors">}}). A "processor" takes in a [stream]({{< ref "#event-streams" >}}) of events as an input and does something to each individual event. What it does to the event is dependent on what kind of processor it is. We'll talk more about different processors later in this section.

But first, let's get started with the fundamentals, the event streams.

## Processors

If you're a bit confused by now don't worry, you're not alone. The word "event processor" is used many places with slightly different semantics. Stream processors are internal constructs and the kinds of event processors we'll be talking about now, however, are strictly the things that the applications will be interacting with.

### Registration

In order to be able to deal with committed events the heads needs to register their processors. The Runtime offers endpoints which initiates the registration of the different processor kinds that we'll be talking more about shortly. The registration part is really important as unregistered processors will not be running. When a head disconnects from the Runtime all of the registered processors will be automatically unregistered. And when it re-connects it wil re-register them again. So processors that has been unregistered will just sit idle in the Runtime doing nothing until they are re-registered again.

### Event Processors

The most pure form of processing events. It will operate on events in a specific stream. That's it. What it does is entirely up to the application, except it cannot create new streams. That is the filter's job.

### Filters

The filter is a special kind of processor that creates a new stream of events from the event log. It has a filter id and can create either a partitioned or unpartitioned stream. The processing in the filter itself is however not partitioned since it can only operate on the event log stream which is an unpartitioned stream.

The filter is powerful because it can create an entirely customized stream of events. It is up to the developer on how to filter the events, during filtering both the content and the metadata of the event is available for the filter to consider. If the filter creates a partitioned stream it also needs to include which partition the event belongs to.

However with great power comes great responsibility. The filters cannot be changed in a way so that it alters the stream it generates the next time it is registered (look at the [rules of the streams]({{< ref "#rules" >}})). If it does, the Runtime would notice it and return a failed registration response to the head that tried to register the filter.

### Event Handlers

The event handler is the most common type of processor. It essentially is a combination of an event processor and a filter. It has an event handler id which will be both the id of both the filter and the event processor.

The event handler's filter is filtering events based on the [Event Type Id]({{< ref "events.md#event-type" >}}) of the event types that the event handler handles. Event handlers can be either partitioned or unpartitioned. Partitioned event handlers uses, by default, the [Event Source Id]({{< ref "event_sourcing.md#event-source-id" >}}) of each event as the partition id. The filter follows the same rules as stated above, that means that if your event handler suddenly stops handling a specific event type that it has already handled or starts handling a new event type that has already ocurred in the event log, the registration of the event handler would fail.

## Multi-tenancy

When registering processors they are registered for every tenant in the Runtime. In terms of processing streams of events, the stream processor is the lowest level unit-of-work. It is what actually does the work, fetches event from the stream, processes the event, etc.  This is important to keep in mind when thinking about the performance and load the the Runtime.

Let's provide an example:

For both the filter and the event processor "processors" only one stream processor is needed. But for event handlers we need two because it consists of both a filter and an event processor. If the Runtime has 10 tenants and the head has registered 20 event handlers we'd end up with a total of 20 x 2 x 10 = 400 stream processors.
