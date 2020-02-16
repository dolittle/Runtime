// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents a version of the Event Log as a natural number, corresponding to the number of events that has been committed to the Event Store.
    /// </summary>
    public class EventLogVersion : ConceptAs<uint>
    {
        /// <summary>
        /// The initial version of the Event Store before any Events are committed.
        /// </summary>
        public static EventLogVersion Initial = 0;

        /// <summary>
        /// Implicitly convert a <see cref="uint"/> to an <see cref="EventLogVersion"/>.
        /// </summary>
        /// <param name="number">The number.</param>
        public static implicit operator EventLogVersion(uint number) => new EventLogVersion { Value = number };
    }
}