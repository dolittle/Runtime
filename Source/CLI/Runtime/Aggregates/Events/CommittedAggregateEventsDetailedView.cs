// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates.Events;

/// <summary>
/// Represents the detailed information for Committed Aggregate Events
/// </summary>
/// <param name="AggregateRootVersion">The Aggregate Root Version.</param>
/// <param name="EventLogSequenceNumber">THe Event Log Sequence Number.</param>
/// <param name="EventType">The Event Type.</param>
/// <param name="Public">Whether the Event is public or not.</param>
/// <param name="Occurred">The date time offset of when the event occurred.</param>
public record CommittedAggregateEventsDetailedView(ulong AggregateRootVersion, ulong EventLogSequenceNumber, string EventType, bool Public, DateTimeOffset Occurred);