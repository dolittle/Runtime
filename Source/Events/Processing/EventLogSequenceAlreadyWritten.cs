// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing;

public class EventLogSequenceAlreadyWritten : Exception
{
    public ulong EventLogSequenceNumber { get; }

    public EventLogSequenceAlreadyWritten(ulong eventLogSequenceNumber) : base($"Event log sequence number {eventLogSequenceNumber} has already been written.")
    {
        EventLogSequenceNumber = eventLogSequenceNumber;
    }
}
