// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that can write an event to a stream in the event store.
    /// </summary>
    public interface IWriteEventToStream
    {
        /// <summary>
        /// Writes an event to a stream in the event store.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        /// <returns>A <see cref="Task"/> representing whether the event was successfully written to the event store.</returns>
        Task<bool> Write(Store.CommittedEvent @event, StreamId streamId);
    }
}