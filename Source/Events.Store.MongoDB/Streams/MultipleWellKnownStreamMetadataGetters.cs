// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams
{
    /// <summary>
    /// Exception that gets thrown when there are multiple well-known stream metadata getters that can get metadata from the <see cref="StreamId" />.
    /// </summary>
    public class MultipleWellKnownStreamMetadataGetters : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleWellKnownStreamMetadataGetters"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public MultipleWellKnownStreamMetadataGetters(StreamId stream)
            : base($"There are multiple instance of {typeof(ICanGetMetadataFromWellKnownStreams).FullName} that can get metadata from stream '{stream}'")
        {
        }
    }
}