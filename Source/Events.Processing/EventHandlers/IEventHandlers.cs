// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Defines a system that knows how to register and start event handlers.
    /// </summary>
    public interface IEventHandlers
    {
        /// <summary>
        /// Registers an event handler.
        /// </summary>
        /// <param name="filterDefinition">The <see cref="IFilterDefinition" />.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="EventHandlerRegistrationResult" />.</returns>
        Task<EventHandlerRegistrationResult> Register(
            TypeFilterWithEventSourcePartitionDefinition filterDefinition,
            IEventProcessor eventProcessor,
            CancellationToken cancellationToken);
    }
}
