// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a system that can wait for events to be written to a stream at a specific <see cref="StreamPosition" />.
    /// </summary>
    public interface IWaitForEventInStream
    {
        /// <summary>
        /// Waits for an event to be at a <see cref="StreamPosition" /> in a stream.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" /> of the event log.</param>
        /// <param name="stream">The <see cref="StreamId" /> of the stream.</param>
        /// <param name="position">The <see cref="StreamPosition" /> of the event.</param>
        /// <param name="timeout">The <see cref="Timeout" /> for waiting.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns></returns>
        Task WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token);
    }
}
