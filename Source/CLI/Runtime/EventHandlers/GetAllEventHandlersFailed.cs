// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Exception that gets thrown when getting all Event Handlers fails.
    /// </summary>
    public class GetAllEventHandlersFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllEventHandlersFailed"/> class.
        /// </summary>
        /// <param name="reason">The reason why getting all Event Handlers failed.</param>
        public GetAllEventHandlersFailed(string reason)
            : base($"Could not get all event handlers because {reason}")
        {
        }
    }
}
