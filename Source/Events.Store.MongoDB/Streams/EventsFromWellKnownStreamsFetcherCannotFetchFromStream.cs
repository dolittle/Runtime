// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when an implementation <see cref="ICanFetchEventsFromWellKnownStreams" /> is
    /// asked to fetch an event from a stream that it cannot fetch from.
    /// </summary>
    public class EventsFromWellKnownStreamsFetcherCannotFetchFromStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventsFromWellKnownStreamsFetcherCannotFetchFromStream"/> class.
        /// </summary>
        /// <param name="fetcher">The <see cref="ICanFetchEventsFromWellKnownStreams" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public EventsFromWellKnownStreamsFetcherCannotFetchFromStream(ICanFetchEventsFromWellKnownStreams fetcher, StreamId stream)
            : base($"{fetcher.GetType().FullName} cannot fetch events from stream '{stream}'")
        {
        }
    }
}