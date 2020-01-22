// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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
        /// <param name="currentVersion">The current version before commit.</param>
        /// <param name="commitVersion">The version of the commit that causes a concurrency conflict.</param>
        public AggregateRootConcurrencyConflict(AggregateRootVersion currentVersion, AggregateRootVersion commitVersion)
            : base($"Current Version is {currentVersion}, tried to commit {commitVersion}")
        {
        }
    }
}