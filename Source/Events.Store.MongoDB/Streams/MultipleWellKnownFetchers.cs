// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when there are multiple well-known stream fetchers that can fetch from the <see cref="StreamId" />.
    /// </summary>
    public class MultipleWellKnownFetchers : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleWellKnownFetchers"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public MultipleWellKnownFetchers(StreamId stream)
            : base($"There are multiple instance of {typeof(ICanFetchFromWellKnownStreams)} that can fetch from stream '{stream}'")
        {
        }
    }
}