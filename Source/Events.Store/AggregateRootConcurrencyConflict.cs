// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when there is a concurrency conflict in an Aggregate Root for a specific <see cref="EventSourceId" />.
    /// </summary>
    public class AggregateRootConcurrencyConflict : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootConcurrencyConflict"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
        /// <param name="aggregateRoot">The aggregate root id.</param>
        /// <param name="currentVersion">The current version before commit.</param>
        /// <param name="commitVersion">The version of the commit that causes a concurrency conflict.</param>
        public AggregateRootConcurrencyConflict(EventSourceId eventSource, ArtifactId aggregateRoot, AggregateRootVersion currentVersion, AggregateRootVersion commitVersion)
            : base($"Tried to commit events to event source {eventSource} on aggregate root {aggregateRoot} with expected aggregate root version {commitVersion}, but current aggregate root version was {currentVersion}. The expected aggregate root version needs to be the same as the current aggregate root version")
        {
        }
    }
}