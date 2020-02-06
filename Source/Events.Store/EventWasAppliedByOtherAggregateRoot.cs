// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IEvent"/> is being used with an Aggregate Root with a different <see cref="Type"/> than it was applied by.
    /// </summary>
    public class EventWasAppliedByOtherAggregateRoot : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventWasAppliedByOtherAggregateRoot"/> class.
        /// </summary>
        /// <param name="eventAggregateRoot">Type <see cref="ArtifactId"/> the Event was applied by.</param>
        /// <param name="aggregateRoot"><see cref="ArtifactId"/> of the Aggregate Root.</param>
        public EventWasAppliedByOtherAggregateRoot(ArtifactId eventAggregateRoot, ArtifactId aggregateRoot)
            : base($"Aggregate Root '{eventAggregateRoot}' from event does not match with '{aggregateRoot}'.")
        {
        }
    }
}
