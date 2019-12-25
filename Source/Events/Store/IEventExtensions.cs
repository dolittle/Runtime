// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;
using Dolittle.PropertyBags;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extensions for <see cref="IEvent" />.
    /// </summary>
    public static class IEventExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="EventEnvelope" /> for this event with the provided <see cref="EventMetadata" />.
        /// </summary>
        /// <param name="event"><see cref="IEvent"/> to create envelope for.</param>
        /// <param name="metadata"><see cref="EventMetadata"/> for the event.</param>
        /// <returns><see cref="EventEnvelope" /> instance.</returns>
        public static EventEnvelope ToEnvelope(this IEvent @event,  EventMetadata metadata)
        {
            return new EventEnvelope(metadata, @event.ToPropertyBag());
        }
    }
}