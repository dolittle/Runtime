// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Aggregates
{
    /// <summary>
    /// Represents an Aggregate Root.
    /// </summary>
    /// <param name="EventSource">The Event Source Id.</param>
    /// <param name="Version">The Aggregate Root Version.</param>
    public record AggregateRootInstance(EventSourceId EventSource, AggregateRootVersion Version);
}
