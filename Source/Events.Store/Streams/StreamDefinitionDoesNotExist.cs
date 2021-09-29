// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Exception that gets thrown when a stream definition does not exist.
    /// </summary>
    public class StreamDefinitionDoesNotExist : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamDefinitionDoesNotExist" /> class.
        /// </summary>
        /// <param name="stream">The stream id.</param>
        /// <param name="scope">The scope id.</param>
        public StreamDefinitionDoesNotExist(StreamId stream, ScopeId scope)
        : base($"Stream definition for stream {stream.Value} in scope {scope.Value} does not exist")
        {
        }
    }
}