// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Exception that gets thrown when getting a partitioned Fetcher for the Event Log stream.
    /// </summary>
    public class CannotGetPartitionedFetcherForEventLog : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotGetPartitionedFetcherForEventLog"/> class.
        /// </summary>
        public CannotGetPartitionedFetcherForEventLog()
            : base($"Cannot get partitioned Fetcher for Event Log")
        {
        }
    }
}
