// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Source"/> does not exist.
    /// </summary>
    public class MissingSource : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingSource"/> class.
        /// </summary>
        /// <param name="source"><see cref="Source"/> that does not exist.</param>
        public MissingSource(Source source)
            : base($"System '{source}' does not exist")
        {
        }
    }
}