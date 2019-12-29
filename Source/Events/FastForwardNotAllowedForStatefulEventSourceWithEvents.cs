// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Exception that gets thrown when an event source is stateful
    /// but there has been an attempt to retrieve it without restoring state by replaying events (fast-forwarding).
    /// </summary>
    public class FastForwardNotAllowedForStatefulEventSourceWithEvents : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FastForwardNotAllowedForStatefulEventSourceWithEvents"/> class.
        /// </summary>
        /// <param name="type">EventSource type.</param>
        public FastForwardNotAllowedForStatefulEventSourceWithEvents(Type type)
            : base($"Cannot fast forward stateful event source of type '{type.AssemblyQualifiedName}' because it is not an initial version.")
        {
        }
    }
}