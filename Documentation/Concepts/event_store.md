---
title: Event Store
description: Introduction to the Event Store
keywords: Overview, Events, Event Store, Event Sourcing
weight: 20
aliases:
    - /runtime/runtime/events/event_store
---
An Event Store is a database optimized for storing [Events]({{< ref "events" >}}) in an [Event Sourced]({{< ref "event_sourcing" >}}) system. The [Runtime]({{< ref "overview" >}}) manages the connections and structure of the stored data. All [Streams]({{< ref "streams" >}}), [Event Handlers & Filters]({{< ref "event_handlers_and_filters" >}}), [Aggregates]({{< ref "aggregates" >}}) and [Event Horizon Subscriptions]({{< ref "event_horizon#subscription" >}}) are being kept track inside the event store.

Events saved to the event store **cannot be changed** or **deleted**. It acts as the record of all events that have happened in the system from the beginning of time. 

Each [Tenant]({{< ref "tenants" >}}) has their own event store database, which is configured in [`resources.json`]({{< ref "docs/reference/runtime/configuration#resourcesjson" >}}).

## Scope

Events that came over the [Event Horizon]({{< ref "event_horizon" >}}) need to be put into a scoped collection so they won't be mixed with the other events from the system.

Scoped collections work the same way as other collections, except you can't have [Public Streams]({{< ref "streams#public-stream" >}}) or [Aggregates]({{< ref "aggregates" >}}).

<!-- The default alert shortcode wouldn't work properly so I copied the alert HTML here -->
<div class="alert alert-info" role="alert">
    <h4 class="alert-heading">Default scope</h4>
    Technically all collections are scoped, with the default scopeID being <code>00000000-0000-0000-0000-000000000000</code>.
    This is left out of the naming to make the event store more readable. When we talk about scoped concepts, we always refer to non-default scopes.
</div>

## Structure of the Event Store

{{< tabs name="collections" >}}
{{% tab name="MongoDB" %}}
This is the structure of the event store implemented in [MongoDB](https://www.mongodb.com/). It includes the following collections in the default [Scope]({{< ref "#scope" >}}):

- `event-log`
- `aggregates`
- `stream-processor-states`
- `stream-definitions`
- `stream-<streamID>`
- `public-stream-<streamID>`

For scoped collections:
- Scoped collections have a `x-scopeID-` prefix in their names
- There is a [`subscription-states`]({{< ref "#subscription-states" >}}) collection for tracking [Subscriptions]({{< ref "event_horizon#subscription" >}})


Following JSON structure examples have each property's [BSON type](https://docs.mongodb.com/manual/reference/bson-types/) as the value.

### `event-log`

The Event Log includes all the [Events]({{< ref "events" >}}) committed to the event store in chronological order. All [streams]({{< ref "streams" >}}) are derived from the event log.

[Aggregate]({{< ref "aggregates" >}}) events have `"wasAppliedByAggregate":  true` set and events coming over the [Event Horizon]({{< ref "event_horizon" >}}) have `"FromEventHorizon": true"` set.

This is the structure of a committed event:
```json
{
    // this it the events EventLogSequenceNumber,
    // which identifies the event uniquely within the event log
    "_id": "decimal",
    "Content": "object",
    // Aggregate metadata
    "Aggregate": {
        "wasAppliedByAggregate": "bool",
        // AggregateRootId
        "TypeId": "UUID",
        // AggregateRoot Version
        "TypeGeneration": "long",
        "Version": "decimal"
    },
    // EventHorizon metadata
    "EventHorizon": {
        "FromEventHorizon": "bool",
        "ExternalEventLogSequenceNumber": "decimal",
        "Received": "date",
        "Concent": "UUID"
    },
    // the committing microservices metadata
    "ExecutionContext": {
        // 
        "Correlation": "UUID",
        "Microservice": "UUID",
        "Tenant": "UUID",
        "Version": "object",
        "Environment": "string",
    },
    // the events metadata
    "Metadata": {
        "Occurred": "date",
        "EventSource": "string",
        // EventTypeId and Generation
        "TypeId": "UUID",
        "TypeGeneration": "long",
        "Public": "bool"
    }
}
```

### `aggregates`

This collection keeps track of all instances of [Aggregates]({{< ref "aggregates#aggregates-in-dolittle" >}}) registered with the Runtime.

```json
{
    "EventSource": "string",
    // the AggregateRootId
    "AggregateType": "UUID",
    "Version": "decimal"
}
```

### `stream`

A [Stream]({{< ref "streams" >}}) contains all the events filtered into it. It's structure is the same as the [`event-log`]({{< ref "#event-log" >}}), with the extra `Partition` property used for [partitions]({{< ref "streams#partitions" >}})

The streams `StreamId` is added to the collections name, eg. a stream with the id of `323bcdb2-5bbd-4f13-a7c3-b19bc2cc2452` would be in a collection called `stream-323bcdb2-5bbd-4f13-a7c3-b19bc2cc2452`.

```json
{
    // same as an Event in the "event-log" + Partition
    "Partition": "string",
}
```

### `public-stream`

The same as a [`stream`]({{< ref "#stream" >}}), except only for [Public Stream]({{< ref "streams#public-vs-private-streams" >}}) with the `public` prefix in collection name. Public streams can only exist on the default [scope]({{< ref "#scope" >}}).

### `stream-definitions`

This collection contains all [Filters]({{< ref "event_handlers_and_filters#filters" >}}) registered with the Runtime.

Filters defined by an [Event Handler]({{< ref "event_handlers_and_filters#event-handlers" >}}) have a type of `EventTypeId`, while other filters have a type of `Remote`.

```json
{
    // id of the Stream the Filter creates
    "_id": "UUID",
    "Partitioned": "bool",
    "Public": "bool",
    "Filter": {
        "Type": "string",
        "Types": [
            // EventTypeIds to filter into the stream
        ]
    }
}
```

### `stream-processor-states`

This collection keeps track of all [Stream Processors]({{< ref "streams#stream-processor" >}}) [Event Processors]({{< ref "streams#event-processor">}}) and their state. Each event processor can be either a [Filter]({{< ref "event_handlers_and_filters#filters" >}}) on an [Event Processor]({{< ref "event_handlers_and_filters#event-handlers" >}}) that handles the events from an event handler.

**Filter**:

```json
{
    "SourceStream": "UUID",
    "EventProcessor": "UUID",
    "Position": "decimal",
    "LastSuccesfullyProcessed": "date",
    // failure tracking information
    "RetryTime": "date",
    "FailureReason": "string",
    "ProcessingAttempts": "int",
    "IsFailing": "bool
}
```

**Event Processor**:

Partitioned streams will have a `FailingPartitions` property for tracking the failing information per partition. It will be empty if there are no failing partitions. The partitions id is the same as the failing events [`EventSourceId`]({{< ref "events#eventsourceid" >}}). As each partition can fail independently, the `"Position"` value can be different for the stream processor at large compared to the failing partitions `"position"`.

```json
{
    "Partitioned": true,
    "SourceStream": "UUID",
    "EventProcessor": "UUID",
    "Position": "decimal",
    "LastSuccessfullyProcessed": "date",
    "FailingPartitions": {
        // for each failing partition
        "<partition-id>": {
            // the position of the failing event in the stream
            "Position": "decimal",
            "RetryTime": "date",
            "Reason": "string",
            "ProcessingAttempts": "int",
            "LastFailed": "date"
        }
    }
}
```


#### `subscription-states`

This collection keeps track of [Event Horizon Subscriptions]({{< ref "event_horizon#subscription" >}}) in a very similar way to [`stream-processor-states`]({{< ref "#stream-processor-states" >}}).
```json
{
    // producers microservice, tenant and stream info
    "Microservice": "UUID",
    "Tenant": "UUID",
    "Stream": "UUID",
    "Partition": "string",
    "Position": "decimal",
    "LastSuccesfullyProcessed": "date",
    "RetryTime": "date",
    "FailureReason": "string",
    "ProcessingAttempts": "int",
    "IsFailing": "bool
}
```

{{% /tab %}}
{{< /tabs >}}

## Commit vs Publish
We use the word `Commit` rather than `Publish` when talking about saving events to the event store. We want to emphasize that it's the event store that is the source of truth in the system. The act of calling [filters/event handlers]({{< ref "event_handlers_and_filters" >}}) comes _after_ the event has been committed to the event store. We also don't publish to any specific [stream]({{< ref "streams" >}}), event handler or [microservice]({{< ref "overview#microservice" >}}). After the event has been committed, it's ready to be picked up by any processor that listens to that type of event.
