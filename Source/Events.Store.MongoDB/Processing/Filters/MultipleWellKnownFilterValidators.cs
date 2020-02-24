// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when there are multiple instances of <see cref="ICanValidateFilterOnWellKnownStreams" /> that can validate filters on a <see cref="StreamId" />.
    /// </summary>
    public class MultipleWellKnownFilterValidators : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleWellKnownFilterValidators"/> class.
        /// </summary>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        public MultipleWellKnownFilterValidators(StreamId stream)
            : base($"There are multiple instance of {typeof(ICanValidateFilterOnWellKnownStreams).FullName} that can validate filters on stream '{stream}'")
        {
        }
    }
}