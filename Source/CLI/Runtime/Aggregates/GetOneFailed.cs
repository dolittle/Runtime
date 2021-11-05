// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Exception that gets thrown when getting one Aggregate Root fails.
    /// </summary>
    public class GetOneFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetOneFailed"/> class.
        /// </summary>
        /// <param name="reason">The reason why the getting one Aggregate Root failed.</param>
        public GetOneFailed(string reason)
            : base($"Could not get one aggregate root because {reason}")
        {
        }
    }
}
