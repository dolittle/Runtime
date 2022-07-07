// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Exception that gets thrown when stream writer tries to write an event with the same event log sequence number to a stream.
/// </summary>
public class EventWithEventLogSequenceNumberAlreadyExistsInStream : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CannotGetPartitionedFetcherForEventLog"/> class.
    /// </summary>
    public EventWithEventLogSequenceNumberAlreadyExistsInStream(EventLogSequenceNumber sequenceNumber, string streamName)
        : base($"Event with event log sequence number {sequenceNumber} is already written to stream {streamName}")
    {
    }
}
