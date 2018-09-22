using System;
using Dolittle.PropertyBags;
using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.Runtime.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Extensions for <see cref="IEvent" />
    /// </summary>
    public static class IEventExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="EventEnvelope" /> for this event with the provided <see cref="EventMetadata" />
        /// </summary>
        public static EventEnvelope ToEnvelope(this IEvent @event,  EventMetadata metatdata)
        {
            return new EventEnvelope(metatdata, @event.ToPropertyBag());
        }
    }
}