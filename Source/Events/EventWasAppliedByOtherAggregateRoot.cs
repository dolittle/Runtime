// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Exception that gets thrown when an event is being used with an Aggregate Root with a different <see cref="Type"/> than it was applied by.
    /// </summary>
    public class EventWasAppliedByOtherAggregateRoot : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventWasAppliedByOtherAggregateRoot"/> class.
        /// </summary>
        /// <param name="eventAggregateRoot">Type <see cref="Type"/> the Event was applied by.</param>
        /// <param name="aggregateRoot"><see cref="Type"/> of the Aggregate Root.</param>
        public EventWasAppliedByOtherAggregateRoot(Type eventAggregateRoot, Type aggregateRoot)
            : base($"Aggregate Root '{eventAggregateRoot.Name}' from event does not match with '{aggregateRoot.Name}'.")
        {
        }
    }
}
