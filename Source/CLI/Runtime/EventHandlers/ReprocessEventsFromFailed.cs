// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Exception that gets thrown when reprocessing events from a position fails.
    /// </summary>
    public class ReprocessEventsFromFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReprocessEventsFromFailed"/> class.
        /// </summary>
        /// <param name="reason">The reason why the reprocessing failed.</param>
        public ReprocessEventsFromFailed(string reason)
            : base($"Could not reprocess events from position because {reason}")
        {
        }
    }
}