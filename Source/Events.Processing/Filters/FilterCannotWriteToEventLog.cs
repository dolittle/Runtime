// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Exception that gets thrown when a filter is targetting the event log stream.
    /// </summary>
    public class FilterCannotWriteToEventLog : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCannotWriteToEventLog"/> class.
        /// </summary>
        public FilterCannotWriteToEventLog()
            : base($"Cannot register filter that targets the event log stream '{StreamId.AllStreamId}'")
        {
        }
    }
}