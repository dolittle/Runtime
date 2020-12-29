---
title: Streams
description: Get an overview of Event Streams
weight: 5
repository: https://github.com/dolittle/Runtime
---

## Event Streams

So, what is a stream? A stream is simply a list with two specific attributes:

* Streams are append-only. Meaning that items can only be put at the very end of the stream, and that the stream is not of a fixed length.
* Items in the stream immutable. The items or their order cannot change.
An event stream is simply a stream of events. Each stream is uniquely identified within an [Event Store]({{< ref "event_sourcing.md#event-store" >}}) by a GUID. An event can belong many streams, and in most cases it will at least belong to two streams (one being the [event log]({{< ref "event_sourcing.md#event-log" >}})).

As streams are append-only, an event can be uniquely identified by its position in a stream, including in the [event log]({{< ref "event_sourcing.md#event-log" >}}).

Event streams are perhaps the most important part of the Dolittle platform. To get a different and more detailed perspective on streams, please read our section on [event sourcing and streams]({{< ref "event_sourcing.md#streams" >}}).

###  Partitions
If we dive deeper into event streams we'll see that we have two types of streams in the Runtime; partitioned and unpartitioned streams.

A partitioned stream is a stream that is split into chunks. These chunks are uniquely identified by a `PartitionId` (GUID). Each item in a partitioned stream can only belong to a single partition.

An unpartitioned stream only has one chunk with a `PartitionId` of `00000000-0000-0000-0000-000000000000`.

There are multiple reasons for partitioning streams. One of the benefits is that it gives a way for the developers to partition their events and the way they are processed in an event handler (we'll talk more about this in [Event Handlers]({{< ref "#event-handlers" >}})). Another reason for having partitions becomes apparent when needing to subscribe to other streams in other microservices. We'll talk more about that in the [Event Horizon]({{< ref "event_horizon.md" >}}) section.

### Stream Processor

A stream processor consists of an event stream and an event processor. It takes in a stream of events, calls the event processor to process the events in order, and keeps track of which events have already been processed. Each stream processor can be seen as the lowest level unit-of-work in regards to streams and they all run at the same time, side by side, in parallel.

The stream processors plays a central role in the Runtime. They enforce the most important rules of Event Sourcing; an event in a stream is not processed twice (unless the stream is being replayed) and that no event in a stream is skipped while processing.

{{< alert title="Resource usage" color="warning">}}
Stream processors in Runtime version 5.2.0 and earlier are particularly resource-heavy, we're working on a fix.
{{< /alert >}}

Stream processors are constructs that are internal to the Runtime and there is no way for the SDK to directly interact with stream processors.

### Dealing with failures
What should happen when a processor fails? We cannot skip faulty events, which means that the event processor has to halt until we can successfully process the event. This problem can be mitigated with a [partitioned]({{< ref "#partitions" >}}) streams because the processing only stops for that single partition. This way we can keep processing the event stream even though one, or several, of the processors fail. The failing processors will keep retrying and will continue processing consequent events normally if they succeed.

#### Event Processor

All stream processors consists of an event stream and an event processor that processes the events in that stream. This is a building block internal in the Runtime. They are similar to, but is not to be confused by, the "front facing" [event processors]({{< ref "#event-processors" >}}). These internal event processors are uniquely identified by an event processor id GUID. Since the streams are also uniquely identified by a stream id we can identify each stream processor by their (stream id, event processor id) pair.

When the processing of an event is completed it returns a processing result back to the stream processor. This results contains information on whether or not the processing succeeded or not. If it did not succeed it will say how many times it has attempted to process that event, whether or not it should retry and how long it will wait until retrying.

### Rules
There are rules on streams to maintain [idempotency](https://en.wikipedia.org/wiki/Idempotence) and the predictability of Runtime. These rules are enforced by the Runtime:

* The ordering of the events cannot change
* Events can only be appended to the end of the stream
* Events cannot be removed from the stream
* A partitioned stream cannot be changed to be unpartitioned and vice versa
