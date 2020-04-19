// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the reason for an event store failure.
    /// </summary>
    public class EventStoreFailureReason : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert <see cref="Guid" /> to <see cref="EventStoreFailureReason" />.
        /// </summary>
        /// <param name="id">The value to convert.</param>
        public static implicit operator EventStoreFailureReason(string id) => new EventStoreFailureReason { Value = id };
    }
}
