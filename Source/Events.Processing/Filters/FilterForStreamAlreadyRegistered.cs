// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when multiple filters are registered for the same <see cref="StreamId" />.
    /// </summary>
    public class FilterForStreamAlreadyRegistered : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterForStreamAlreadyRegistered"/> class.
        /// </summary>
        /// <param name="streamId">The <see cref="StreamId" />.</param>
        public FilterForStreamAlreadyRegistered(StreamId streamId)
            : base($"There has already been registered a filter for stream '{streamId}'")
        {
        }
    }
}