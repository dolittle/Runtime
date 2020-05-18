// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when an inconsistency is detected in the event store at runtime.
    /// </summary>
    public class EventStoreConsistencyError : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreConsistencyError"/> class.
        /// </summary>
        /// <param name="cause">The cause of the persistence problem exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/>.</param>
        public EventStoreConsistencyError(string cause, Exception innerException)
            : base(cause, innerException)
        {
        }
    }
}
