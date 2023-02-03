// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines a system that can publish a notification when an event has been written to a <see cref="StreamPosition" />.
/// </summary>
public interface INotifyOfStreamEvents
{
    /// <summary>
    /// Notifies that an event has been written to the event store to a stream and position.
    /// </summary>
    /// <param name="scope">The <see cref="ScopeId" /> of the event log.</param>
    /// <param name="stream">The <see cref="StreamId" /> of the stream.</param>
    /// <param name="position">The <see cref="StreamPosition" /> of the event.</param>
    void NotifyForEvent(ScopeId scope, StreamId stream, ProcessingPosition position);
}
