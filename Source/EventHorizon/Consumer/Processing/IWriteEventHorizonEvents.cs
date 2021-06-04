// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing
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
        /// <param name="consentId">The <see cref="ConsentId" />.</param>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The task.</returns>
        Task Write(CommittedEvent @event, ConsentId consentId, ScopeId scope, CancellationToken cancellationToken);
    }
}