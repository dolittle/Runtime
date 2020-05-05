// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when an implementation <see cref="ICanFetchFromWellKnownStreams" /> is
    /// asked to fetch an event from a stream that it cannot fetch from.
    /// </summary>
    public class FetcherForWellKnownStreamCannotFetchFromStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FetcherForWellKnownStreamCannotFetchFromStream"/> class.
        /// </summary>
        /// <param name="fetcher">The <see cref="ICanFetchFromWellKnownStreams" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public FetcherForWellKnownStreamCannotFetchFromStream(ICanFetchFromWellKnownStreams fetcher, StreamId stream)
            : base($"{fetcher.GetType()} cannot fetch events from stream '{stream}'")
        {
        }
    }
}