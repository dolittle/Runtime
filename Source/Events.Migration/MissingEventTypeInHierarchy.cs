// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IEvent">Event</see> in an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see>
    /// has not been registered as an <see cref="IEvent">Event</see>.
    /// </summary>
    public class MissingEventTypeInHierarchy : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingEventTypeInHierarchy"/> class.
        /// </summary>
        /// <param name="typeName">Name of type.</param>
        public MissingEventTypeInHierarchy(string typeName)
            : base($"Cannot find an event migration hierarchy with the event named '{typeName}'.")
        {
        }
    }
}