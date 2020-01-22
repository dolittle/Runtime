// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the identification of an event handler.
    /// </summary>
    public class EventHandlerId : ConceptAs<Guid>
    {
        /// <summary>
        /// Represents a not set <see cref="EventHandlerId"/>.
        /// </summary>
        public static EventHandlerId NotSet = Guid.Empty;

        /// <summary>
        /// Implicitly convert from a <see cref="Guid"/> to a <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="handlerId"><see cref="Guid"/> representation.</param>
        public static implicit operator EventHandlerId(Guid handlerId) => new EventHandlerId { Value = handlerId };

        /// <summary>
        /// Creates a new instance of <see cref="EventHandlerId"/> with a unique id.
        /// </summary>
        /// <returns>A new <see cref="EventHandlerId"/>.</returns>
        public static EventHandlerId New()
        {
            return new EventHandlerId { Value = Guid.NewGuid() };
        }
    }
}
