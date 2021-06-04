// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Lifecycle;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHandlers"/>.
    /// </summary>
    [Singleton]
    public class EventHandlers : IEventHandlers
    {
        ConcurrentBag<EventHandler> _eventHandlers = new();

        /// <inheritdoc/>
        public IEnumerable<EventHandler> All => _eventHandlers.ToArray();

        /// <inheritdoc/>
        public void Register(EventHandler eventHandler) => _eventHandlers.Add(eventHandler);

        /// <inheritdoc/>
        public void Unregister(EventHandler eventHandler) => _eventHandlers = new(_eventHandlers.Except(new[] { eventHandler }));
    }
}
