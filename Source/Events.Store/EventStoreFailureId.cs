// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the unique identifier of an event store failure type.
    /// </summary>
    public class EventStoreFailureId : ConceptAs<Guid>
    {
        /// <summary>
        /// Implicitly convert <see cref="Guid" /> to <see cref="EventStoreFailureId" />.
        /// </summary>
        /// <param name="id">The value to convert.</param>
        public static implicit operator EventStoreFailureId(Guid id) => new EventStoreFailureId { Value = id };
    }
}
