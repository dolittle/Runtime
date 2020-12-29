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

## Stream Partition

If we go deeper down the hole that is event streams we'll see that we have two types of streams in the Runtime; partitioned and unpartitioned streams.

A partitioned stream is a stream that is split into chunks. These chunks are uniquely identified by a partition id (GUID). Each item in a partitioned stream can only belong to a single partition.

While an unpartitioned stream is, as you'd probably guess, a stream that is not split into chunks. It only consists of one big chunk if you will.

There are multiple reasons for partitioning streams. One of the benefits is that it gives a way for the developers to naturally partition their events and the way they are processed in an event handler (we'll talk more about this [later]({{< ref "#event-handlers" >}})). Another reason for having partitions becomes apparent when needing to subscribe to other streams in other microservices. We'll talk more about that in the section about the [Event Horizon]({{< ref "event_horizon.md" >}})

## Stream Processor

Before talking about the actual event [processors]({{< ref "#processors" >}}) we should just touch upon the main building block of the event processors, the stream processor. Stream processors are constructs that are internal to the Runtime and there is no way for applications to directly interact with stream processors. It is almost the same as an event processor on a conceptual level; it takes in as input a stream of events and processes each event in order. But it does not know how to process the event, that's the event processor's job. Each stream processor can be seen as the lowest level unit-of-work in regards to streams in the Runtime and they all run at the same time, side by side, in parallel.

They do have a resource cost associated with them (and right now as of Runtime version 5.2.0 they are very resource heavy). That cost comes due to the fact that the stream processor needs to get the first unprocessed event in the stream and then send it over to the event processor for processing. And while there are no new events to process it needs to wait.

Since the stream processors work on streams, and streams can be partitioned or unpartitioned, we naturally need two different kinds of stream processors. They operate slightly different, though the difference is of significant importance.
The difference comes into play when the processing of an event fails for any reason. What should then happen? We cannot really just skip that faulty event so that means that the processing of events would come to a halt and continue until we can successfully process the faulty event again. The severity of halting the processing of events can be quite significant, however this is mitigated in partitioned streams because the processing only stops for that single partition. So that's one of the reasons why you'd want to partition event streams, to be able to try continue processing the event stream even though one, or several, of the events cannot be processed right now. Remember that an event in a stream can only belong to one partition? We can continue processing events as long as the event to process does not belong to a partition that is failing. The faulty partitions won't necessarily be halted forever we can retry processing again at any time and if it succeeds the processing of that partition will continue as normal again.

The stream processors plays a central role in the Runtime, and without it the Runtime would be completely useless. They also enforce the most important rules of the Event Sourced Runtime; that an event in a stream is not processed twice (unless the stream is being replayed) and that no event in a stream is skipped while processing.

### Event Processor

All stream processors consists of an event stream and an event processor that processes the events in that stream. This is a building block internal in the Runtime. They are similar to, but is not to be confused by, the "front facing" [event processors]({{< ref "#event-processors" >}}). These internal event processors are uniquely identified by an event processor id GUID. Since the streams are also uniquely identified by a stream id we can identify each stream processor by their (stream id, event processor id) pair.

When the processing of an event is completed it returns a processing result back to the stream processor. This results contains information on whether or not the processing succeeded or not. If it did not succeed it will say how many times it has attempted to process that event, whether or not it should retry and how long it will wait until retrying.

## Rules

In order to maintain the [idempotency](https://en.wikipedia.org/wiki/Idempotence) of your event processors and the predictability of the Runtime we need to establish a couple of rules on streams. Some of the rules are directly related to the attributes of a stream.

* The ordering of the events cannot change
* Events can only be appended to the end of the stream
* Events cannot be removed from the stream
* A partitioned stream cannot be changed to be unpartitioned and vice versa

The Runtime will enforce these rules. You're not allowed to do anything that breaks these rules.
