// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when there is an error when trying to save to the event store.
    /// </summary>
    public class EventStorePersistenceError : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStorePersistenceError"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public EventStorePersistenceError(string message)
            : base(message)
        {
        }
    }
}