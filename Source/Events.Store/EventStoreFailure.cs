// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Holds the <see cref="EventStoreFailureReason" /> and <see cref="EventStoreFailureId" /> related to a failure of an event store operation.
    /// </summary>
    public class EventStoreFailure
    {
        /// <summary>
        /// Gets the <see cref="EventStoreFailureId" />.
        /// </summary>
        public EventStoreFailureId Id { get; }

        /// <summary>
        /// Gets the <see cref="EventStoreFailureReason" />.
        /// </summary>
        public EventStoreFailureReason Reason { get; }
    }
}
