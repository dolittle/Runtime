// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when there is a concurrency conflict in an Event Stream for a specific <see cref="IEventSource" />.
    /// </summary>
    public class EventSourceConcurrencyConflict : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceConcurrencyConflict"/> class.
        /// </summary>
        /// <param name="message">A message describing the exception.</param>
        public EventSourceConcurrencyConflict(string message)
            : base(message)
        {
        }
    }
}