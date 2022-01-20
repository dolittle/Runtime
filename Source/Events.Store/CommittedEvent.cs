// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Execution;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represent an Event that is committed to the Event Store.
/// </summary>
public class CommittedEvent : Event
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommittedEvent"/> class.
    /// </summary>
    /// <param name="eventLogSequenceNumber">The sequence number of the Event Log the Event was committed to.</param>
    /// <param name="occurred">The <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.</param>
    /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
    /// <param name="executionContext">The current <see cref="ExecutionContext" /> when this event was committed.</param>
    /// <param name="type">The <see cref="Artifact"/> representing the type of the Event.</param>
    /// <param name="isPublic">Whether this Event is public.</param>
    /// <param name="content">The content of the Event represented as a JSON-encoded <see cref="string"/>.</param>
    public CommittedEvent(
        EventLogSequenceNumber eventLogSequenceNumber,
        DateTimeOffset occurred,
        EventSourceId eventSource,
        ExecutionContext executionContext,
        Artifact type,
        bool isPublic,
        string content)
        : base(eventSource, type, isPublic, content)
    {
        EventLogSequenceNumber = eventLogSequenceNumber;
        Occurred = occurred;
        ExecutionContext = executionContext;
    }

    /// <summary>
    /// Gets the sequence number of the Event Log the Event was committed to.
    /// </summary>
    public EventLogSequenceNumber EventLogSequenceNumber { get; }

    /// <summary>
    /// Gets the <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.
    /// </summary>
    public DateTimeOffset Occurred { get; }

    /// <summary>
    /// Gets the <see cref="ExecutionContext" /> that committed this event.
    /// </summary>
    public ExecutionContext ExecutionContext { get; }
}