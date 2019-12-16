// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents the error when an <see cref="SingleEventTypeEventStream" /> is created with events from more than one Event Type.
    /// </summary>
    public class MultipleEventTypesInSingleEventTypeEventStream : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleEventTypesInSingleEventTypeEventStream"/> class.
        /// </summary>
        public MultipleEventTypesInSingleEventTypeEventStream()
        {
        }
    }
}