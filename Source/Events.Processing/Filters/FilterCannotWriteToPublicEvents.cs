// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when a filter is targetting the public events stream.
    /// </summary>
    public class FilterCannotWriteToPublicEvents : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCannotWriteToPublicEvents"/> class.
        /// </summary>
        public FilterCannotWriteToPublicEvents()
            : base($"A filter cannot be registered to write to the public events stream '{StreamId.PublicEventsId}'")
        {
        }
    }
}