// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="AbstractEventsToWellKnownStreamsWriter" /> is instantiated with the event log <see cref="StreamId" />.
    /// </summary>
    public class CannotCreateWellKnownStreamsWriterOnEventLog : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotCreateWellKnownStreamsWriterOnEventLog"/> class.
        /// </summary>
        public CannotCreateWellKnownStreamsWriterOnEventLog()
            : base($"Cannot create a stream writer that can write to the event log stream '{StreamId.AllStreamId}'")
        {
        }
    }
}