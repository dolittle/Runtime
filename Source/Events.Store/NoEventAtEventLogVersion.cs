// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when there is no event in the event log at a specific event log version.
    /// </summary>
    public class NoEventAtEventLogVersion : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoEventAtEventLogVersion"/> class.
        /// </summary>
        /// <param name="eventLogVersion">The <see cref="EventLogVersion" />.</param>
        public NoEventAtEventLogVersion(EventLogVersion eventLogVersion)
            : base($"There was no event in the event log at event log version {eventLogVersion}")
        {
        }
    }
}
