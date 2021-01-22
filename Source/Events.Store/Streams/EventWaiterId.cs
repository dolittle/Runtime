// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Represents the unique identifier for an <see cref="EventWaiter" />.
    /// </summary>
    public class EventWaiterId : Value<EventWaiterId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventWaiterId"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public EventWaiterId(ScopeId scope, StreamId stream)
        {
            Scope = scope;
            Stream = stream;
        }

        /// <summary>
        /// Gets the <see cref="ScopeId" />.
        /// </summary>
        public ScopeId Scope { get; }

        /// <summary>
        /// Gets the <see cref="StreamId" />.
        /// </summary>
        public StreamId Stream { get; }
    }
}
