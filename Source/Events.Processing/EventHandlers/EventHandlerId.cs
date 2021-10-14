// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents a unique identifier for an event handler.
    /// </summary>
    public record EventHandlerId(Guid Value) : ConceptAs<Guid>(Value)
    {
        /// <summary>
        /// Implicitly convert from <see cref="Guid"/> to <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="identifier"><see cref="Guid"/> representation.</param>
        public static implicit operator EventHandlerId(Guid identifier) => new(identifier);

        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="EventHandlerId"/>.
        /// </summary>
        /// <param name="identifier"><see cref="string"/> representation.</param>
        public static implicit operator EventHandlerId(string identifier) => Guid.Parse(identifier);
    }
}