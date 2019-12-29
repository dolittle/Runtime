// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Applications.Configuration
{
    /// <summary>
    /// Exception that gets thrown when there is no configuration for the bounded context.
    /// </summary>
    public class MissingBoundedContextConfiguration : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingBoundedContextConfiguration"/> class.
        /// </summary>
        /// <param name="path">Search path.</param>
        public MissingBoundedContextConfiguration(string path)
            : base($"Missing configuration for the bounded context - looking for file 'bounded-context.json' at path {path}")
        {
        }
    }
}