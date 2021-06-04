// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Defines a system for holding all current event
    /// </summary>
    public interface IEventHandlers
    {
        /// <summary>
        /// Gets all the registered event handlers for a specific.
        /// </summary>
        IEnumerable<EventHandler> All { get; }

        /// <summary>
        /// Register an <see cref="EventHandler"/>.
        /// </summary>
        /// <param name="eventHandler"><see cref="EventHandler"/> to register.</param>
        void Register(EventHandler eventHandler);

        /// <summary>
        /// Register an <see cref="EventHandler"/>.
        /// </summary>
        /// <param name="eventHandler"><see cref="EventHandler"/> to register.</param>
        void Unregister(EventHandler eventHandler);
    }
}
