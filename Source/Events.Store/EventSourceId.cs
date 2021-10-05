// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the identification of an event source.
    /// </summary>
    public record EventSourceId(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// A static singleton instance to represent a "NotSet" <see cref="EventSourceId" />.
        /// </summary>
        public static readonly EventSourceId NotSet = Guid.Empty.ToString();

        /// <summary>
        /// Implicitly convert from a <see cref="string"/> to an <see cref="EventSourceId"/>.
        /// </summary>
        /// <param name="eventSourceId">EventSourceId as <see cref="string"/>.</param>
        public static implicit operator EventSourceId(string eventSourceId) => new(eventSourceId);
    }
}
