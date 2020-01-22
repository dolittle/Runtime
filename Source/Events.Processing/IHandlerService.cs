// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a service which can handle an event.
    /// </summary>
    public interface IHandlerService
    {
        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="event">The <see cref="CommittedEventEnvelope" />.</param>
        /// <param name="eventProcessorId">The <see cref="EventProcessorId" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of handling an event.</returns>
        Task<IHandlerResult> Handle(CommittedEventEnvelope @event, EventProcessorId eventProcessorId);
    }
}