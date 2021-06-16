// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store.MongoDB.Aggregates
{
    /// <summary>
    /// Exception that gets thrown when attempting to decrease the version of an aggregate root instance.
    /// </summary>
    public class NextAggregateRootVersionMustBeGreaterThanCurrentVersion : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NextAggregateRootVersionMustBeGreaterThanCurrentVersion"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRoot">The aggregate root id.</param>
        /// <param name="currentVersion">The current <see cref="AggregateRootVersion"/> of the aggregate root instance.</param>
        /// <param name="nextVersion">The <see cref="AggregateRootVersion"/> that was attempted to set on the aggregate root instance.</param>
        public NextAggregateRootVersionMustBeGreaterThanCurrentVersion(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion currentVersion, AggregateRootVersion nextVersion)
            : base($"Next aggregate root version of aggregate root {aggregateRoot} with event source {eventSource} must be greater than '{currentVersion}', but got '{nextVersion}'.")
        {
        }
    }
}
