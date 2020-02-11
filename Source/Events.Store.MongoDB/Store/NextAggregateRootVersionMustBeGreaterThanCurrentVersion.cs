// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    /// <summary>
    /// Exception that gets thrown when attempting to decrease the version of an aggregate root instance.
    /// </summary>
    public class NextAggregateRootVersionMustBeGreaterThanCurrentVersion : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NextAggregateRootVersionMustBeGreaterThanCurrentVersion"/> class.
        /// </summary>
        /// <param name="currentVersion">The current <see cref="AggregateRootVersion"/> of the aggregate root instance.</param>
        /// <param name="nextVersion">The <see cref="AggregateRootVersion"/> that was attempted to set on the aggregate root instance.</param>
        public NextAggregateRootVersionMustBeGreaterThanCurrentVersion(AggregateRootVersion currentVersion, AggregateRootVersion nextVersion)
            : base($"Next Aggregate Root instance version must be greater than '{currentVersion}', got '{nextVersion}'.")
        {
        }
    }
}