// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Defines a system that can write events from an event horizon.
    /// </summary>
    public interface IWriteEventHorizonEvents
    {
        /// <summary>
        /// Writes a received event.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEvent" />.</param>
        /// <param name="eventHorizon">The <see cref="EventHorizon" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task.</returns>
        Task Write(CommittedEvent @event, EventHorizon eventHorizon, CancellationToken cancellationToken = default);
    }
}