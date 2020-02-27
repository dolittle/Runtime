// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when there are multiple well-known stream event fetchers that can fetch event from the <see cref="StreamId" />.
    /// </summary>
    public class MultipleWellKnownStreamEventFetchers : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleWellKnownStreamEventFetchers"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public MultipleWellKnownStreamEventFetchers(StreamId stream)
            : base($"There are multiple instance of {typeof(ICanFetchEventsFromWellKnownStreams).FullName} that can fetch event from stream '{stream}'")
        {
        }
    }
}