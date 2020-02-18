// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing
{
    /// <summary>
    /// Exception that gets thrown when an implementation of <see cref="ICanWriteEventsToWellKnownStreams" />
    /// is asked to write an event to a stream that it cannot write to.
    /// </summary>
    public class EventsToWellKnownStreamsWriterCannotWriteToStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventsToWellKnownStreamsWriterCannotWriteToStream"/> class.
        /// </summary>
        /// <param name="writer">The <see cref="ICanWriteEventsToWellKnownStreams" />.</param>
        /// <param name="streamId">The <see cref="StreamId" /> that it can't write to.</param>
        public EventsToWellKnownStreamsWriterCannotWriteToStream(ICanWriteEventsToWellKnownStreams writer, StreamId streamId)
            : base($"{writer.GetType().FullName} cannot write events to stream '{streamId}'")
        {
        }
    }
}