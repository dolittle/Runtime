// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when a sequence of events are not valid for the Aggregate Root it is being used with.
    /// </summary>s
    public class AggregateRootVersionIsOutOfOrder : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootVersionIsOutOfOrder"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRoot">The aggregate root id.</param>
        /// <param name="eventVersion">The <see cref="AggregateRootVersion"/> the Event was applied by.</param>
        /// <param name="expectedVersion">Expected <see cref="AggregateRootVersion"/>.</param>
        public AggregateRootVersionIsOutOfOrder(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion eventVersion, AggregateRootVersion expectedVersion)
            : base($"Aggregate Root version for event source {eventSource} on aggregate root {aggregateRoot} is out of order. Version '{eventVersion}' from event does not match '{expectedVersion}'")
        {
        }
    }
}
