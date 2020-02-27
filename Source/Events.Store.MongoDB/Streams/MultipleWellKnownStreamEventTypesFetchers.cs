// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when there are multiple well-known stream event types fetchers that can fetch event types from the <see cref="StreamId" />.
    /// </summary>
    public class MultipleWellKnownStreamEventTypesFetchers : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleWellKnownStreamEventTypesFetchers"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public MultipleWellKnownStreamEventTypesFetchers(StreamId stream)
            : base($"There are multiple instance of {typeof(ICanFetchEventTypesFromWellKnownStreams).FullName} that can fetch event types from stream '{stream}'")
        {
        }
    }
}