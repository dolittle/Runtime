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
        /// <param name="currentVersion">The current version before commit.</param>
        /// <param name="commitVersion">The version of the commit that causes a concurrency conflict.</param>
        public EventSourceConcurrencyConflict(EventSourceVersion currentVersion, EventSourceVersion commitVersion)
            : base($"Current Version is {currentVersion}, tried to commit {commitVersion}")
        {
        }
    }
}