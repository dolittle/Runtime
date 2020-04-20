// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when an implementation of <see cref="ICanFetchEventTypesFromWellKnownStreams" /> is
    /// asked to fetch event types from a stream that it cannot fetch from.
    /// </summary>
    public class EventTypesFromWellKnownStreamsFetcherCannotFetchFromStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTypesFromWellKnownStreamsFetcherCannotFetchFromStream"/> class.
        /// </summary>
        /// <param name="fetcher">The <see cref="ICanFetchEventsFromWellKnownStreams" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public EventTypesFromWellKnownStreamsFetcherCannotFetchFromStream(ICanFetchEventTypesFromWellKnownStreams fetcher, StreamId stream)
            : base($"{fetcher.GetType().FullName} cannot fetch event types from stream '{stream}'")
        {
        }
    }
}