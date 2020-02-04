// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.TimeSeries.Identity
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="Tag"/> is missing in a <see cref="Source"/>.
    /// </summary>
    public class MissingTagInSource : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingTagInSource"/> class.
        /// </summary>
        /// <param name="source"><see cref="Source"/> the <see cref="Tag"/> is missing.</param>
        /// <param name="tag"><see cref="Tag"/> that is missing.</param>
        public MissingTagInSource(Source source, Tag tag)
            : base($"Tag '{tag}' is missing in control system '{source}'")
        {
        }
    }
}