// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Exception that gets thrown we a projection operation fails.
    /// </summary>
    public class ProjectionFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionFailed"/> class.
        /// </summary>
        /// <param name="reason">The reason for the failure.</param>
        public ProjectionFailed(string reason)
            : base($"Projection failed because ${reason}")
        {
        }
    }
}
