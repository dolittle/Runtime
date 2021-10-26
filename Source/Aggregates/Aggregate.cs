// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Aggregates
{
    public record Aggregate(EventSourceId EventSource, AggregateRootVersion Version);
}
