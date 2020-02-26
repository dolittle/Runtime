// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Exception that gets thrown when a the <see cref="IWriteEventsToStreams" /> tries to write an event twice to a stream.
    /// </summary>
    public class EventAlreadyWrittenToStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventAlreadyWrittenToStream"/> class.
        /// </summary>
        /// <param name="eventType">The event.</param>
        /// <param name="eventLogSequenceNumber">The event log sequence number of the event.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public EventAlreadyWrittenToStream(ArtifactId eventType, EventLogSequenceNumber eventLogSequenceNumber, StreamId stream)
            : base($"Event '{eventType}' with event log sequence number '{eventLogSequenceNumber}' has already been written to stream '{stream}'")
        {
        }
    }
}