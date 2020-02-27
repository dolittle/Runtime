// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Exception that gets thrown when something is attempting to fetch a range of events from an event horizon.
    /// </summary>
    public class CannotFetchRangeOfEventsFromEventHorizon : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotFetchRangeOfEventsFromEventHorizon"/> class.
        /// </summary>
        public CannotFetchRangeOfEventsFromEventHorizon()
            : base("Cannot fetch a range of events from event horizon")
        {
        }
    }
}