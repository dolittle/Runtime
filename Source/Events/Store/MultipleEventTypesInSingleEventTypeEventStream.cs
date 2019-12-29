// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="SingleEventTypeEventStream" /> is created with events from more than one Event Type.
    /// </summary>
    public class MultipleEventTypesInSingleEventTypeEventStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleEventTypesInSingleEventTypeEventStream"/> class.
        /// </summary>
        public MultipleEventTypesInSingleEventTypeEventStream()
            : base("There are multiple different event types in an event stream purposed for only one type.")
        {
        }
    }
}