// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Exception that gets thrown when attempting to fetch a stream of aggregate events with specific event types when there are no event types in the request.
/// </summary>
public class CannotFetchAggregateEventsWithNoEventTypes : Exception
{
    public CannotFetchAggregateEventsWithNoEventTypes(EventSourceId eventSource, ArtifactId aggregateRoot)
        : base($"Cannot fetch aggregate events for aggregate with aggregate root {aggregateRoot} and event source {eventSource} when filtering on no event types")
    {
    }
}
