/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a system that knows about <see cref="IEventProcessor">event processors</see>
    /// </summary>
    public interface IEventProcessors
    {
        /// <summary>
        /// Gets all the available <see cref="IEventProcessor">event processors</see>
        /// </summary>
        IEnumerable<IEventProcessor>    All { get; }

        /// <summary>
        /// Process an <see cref="IEvent">event</see>
        /// </summary>
        /// <param name="committedEvents">The <see cref="Store.CommittedEventStream"/> holding committed events</param>
        void Process(Store.CommittedEventStream committedEvents);
    }
}