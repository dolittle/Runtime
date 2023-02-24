// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines a system that can wait for events to be written to a public stream at a specific <see cref="StreamPosition" />.
/// </summary>
public interface IWaitForEventInPublicStream
{
    /// <summary>
    /// Waits for an event to be at a <see cref="StreamPosition" /> in a public stream.
    /// </summary>
    /// <param name="stream">The <see cref="StreamId" /> of the stream.</param>
    /// <param name="position">The <see cref="StreamPosition" /> of the event.</param>
    /// <param name="timeout">The <see cref="Timeout" /> for waiting.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
    Task WaitForEvent(StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token);

    /// <summary>
    /// Waits for an event to be at a <see cref="StreamPosition" /> in a publis tream with a default timeout of 1 min.
    /// </summary>
    /// <param name="stream">The <see cref="StreamId" /> of the stream.</param>
    /// <param name="position">The <see cref="StreamPosition" /> of the event.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>The <see cref="Task" /> representing the asynchronous operation.</returns>
    Task WaitForEvent(StreamId stream, StreamPosition position, CancellationToken token);
}
