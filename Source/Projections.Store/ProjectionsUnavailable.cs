// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Projections.Store
{
    /// <summary>
    /// Exception that gets thrown when the projections store is unavailable.
    /// </summary>
    public class ProjectionsUnavailable : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionsUnavailable"/> class.
        /// </summary>
        /// <param name="cause">The cause of why the event store is unavailable.</param>
        /// <param name="innerException">The inner <see cref="Exception"/>.</param>
        public ProjectionsUnavailable(string cause, Exception innerException)
            : base(cause, innerException)
        {
        }
    }
}