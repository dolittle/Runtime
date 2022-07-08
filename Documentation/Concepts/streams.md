---
title: Streams
description: Get an overview of Event Streams
weight: 5
repository: https://github.com/dolittle/Runtime
---

So, what is a stream? A stream is simply a list with two specific attributes:

* Streams are append-only. Meaning that items can only be put at the very end of the stream, and that the stream is not of a fixed length.
* Items in the stream immutable. The items or their order cannot change.
An event stream is simply a stream of events. Each stream is uniquely identified within an [Event Store]({{< ref "event_sourcing.md#event-store" >}}) by a GUID. An event can belong many streams, and in most cases it will at least belong to two streams (one being the [event log]({{< ref "event_sourcing.md#event-log" >}})).

As streams are append-only, an event can be uniquely identified by its position in a stream, including in the [event log]({{< ref "event_sourcing.md#event-log" >}}).

Event streams are perhaps the most important part of the Dolittle platform. To get a different and more detailed perspective on streams, please read our section on [event sourcing and streams]({{< ref "event_sourcing.md#streams" >}}).

## Rules

There are rules on streams to maintain [idempotency](https://en.wikipedia.org/wiki/Idempotence) and the predictability of Runtime. These rules are enforced by the Runtime:

* The ordering of the events cannot change
* Events can only be appended to the end of the stream
* Events cannot be removed from the stream
* A partitioned stream cannot be changed to be unpartitioned and vice versa

## Partitions

If we dive deeper into event streams we'll see that we have two types of streams in the Runtime; partitioned and unpartitioned streams.

A partitioned stream is a stream that is split into chunks. These chunks are uniquely identified by a `PartitionId` (string). Each item in a partitioned stream can only belong to a single partition.

An unpartitioned stream only has one chunk with a `PartitionId` of `00000000-0000-0000-0000-000000000000`.

There are multiple reasons for partitioning streams. One of the benefits is that it gives a way for the developers to partition their events and the way they are processed in an [Event Handler]({{< ref "#event-handlers" >}}). Another reason for having partitions becomes apparent when needing to subscribe to other streams in other microservices. We'll talk more about that in the [Event Horizon]({{< ref "event_horizon.md" >}}) section.

## Public vs Private Streams

There are two different types of event streams; public and private. Private streams are exposed within their [Tenant]({{< ref "#multi-tenancy" >}}) and public streams are additionally exposed to other microservices.
Through the [Event Horizon]({{< ref "event_horizon" >}}) other microservices can subscribe to your public streams. Using a [public filter]({{< ref "event_handlers_and_filters.md#public-filters" >}}) you can filter out [public events]({{< ref "events.md#public-vs-private" >}}) to public streams.

## Stream Processor

A stream processor consists of an event stream and an event processor. It takes in a stream of events, calls the event processor to process the events in order, keeps track of which events have already been processed, which have failed and when to retry. Each stream processor can be seen as the lowest level unit-of-work in regards to streams and they all run at the same time, side by side, in parallel.

Since the streams are also uniquely identified by a stream id we can identify each stream processor by their `SourceStream, EventProcessor` pairing.

```csharp
// structure of a StreamProcessor
StreamProcessor {
    SourceStream Guid
    EventProcessor Guid
    // the next event to be processed
    Position int
    // for keeping track of failures and retry attempts
    LastSuccesfullyProcessed DateTime
    RetryTime DateTime
    FailureReason string
    ProcessingAttempts int
    IsFailing bool
}
```

The stream processors play a central role in the Runtime. They enforce the most important rules of Event Sourcing; an event in a stream is not processed twice (unless the stream is being replayed) and that no event in a stream is skipped while processing.

Stream processors are constructs that are internal to the Runtime and there is no way for the SDK to directly interact with stream processors.

### Dealing with failures

What should happen when a processor fails? We cannot skip faulty events, which means that the event processor has to halt until we can successfully process the event. This problem can be mitigated with a [partitioned]({{< ref "#partitions" >}}) stream because the processing only stops for that single partition. This way we can keep processing the event stream even though one, or several, of the partitions fail. The stream processor will at some point retry processing the failing partitions and continue normally if it succeeds.

### Event Processors

There are 2 different types of event processors:

- Filters that can create new streams
- Processors that process the event in the user's code

These are defined by the user with [Event Handlers & Filters]({{< ref "event_handlers_and_filters#event-processors" >}}).

When the processing of an event is completed it returns a processing result back to the stream processor. This result contains information on whether or not the processing succeeded or not. If it did not succeed it will say how many times it has attempted to process that event, whether or not it should retry and how long it will wait until retrying.

### Multi-tenancy

When registering [processors]({{< ref "event_handlers_and_filters.md" >}}) they are registered for every tenant in the Runtime, resulting in every tenant having their own copy of the stream processor.

Formula for calculating the total number of stream processors created:
```
(((2 x event handlers) + filters) x tenants)  + event horizon subscriptions = stream processors
```

Let's provide an example:

For both the filter and the event processor "processors" only one stream processor is needed. But for event handlers we need two because it consists of both a filter and an event processor. If the Runtime has 10 tenants and the head has registered 20 event handlers we'd end up with a total of 20 x 2 x 10 = 400 stream processors.
