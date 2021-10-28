// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.Events
{
    /// <summary>
    /// Represents the simple information for Committed Aggregate Events
    /// </summary>
    /// <param name="AggregateRootVersion">The Aggregate Root Version.</param>
    /// <param name="EventLogSequenceNumber">THe Event Log Sequence Number.</param>
    /// <param name="EventType">The Event Type.</param>
    public record CommittedAggregateEventsSimpleView(ulong AggregateRootVersion, ulong EventLogSequenceNumber, Guid EventType);
}
