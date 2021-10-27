// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Represents an Aggregate.
    /// </summary>
    /// <param name="Root">The Aggregate Root.</param>
    /// <param name="EventSource">The Event Source Id.</param>
    /// <param name="Version">The current Aggregate Root Version.</param>
    public record Aggregate(AggregateRoot Root, EventSourceId EventSource, AggregateRootVersion Version);
}
