// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Represents an exceptional situation where an <see cref="IEvent">Event</see> in an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see>
    /// has not been registered as an <see cref="IEvent">Event</see>.
    /// </summary>
    public class UnregisteredEventException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisteredEventException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public UnregisteredEventException(string message)
            : base(message)
        {
        }
    }
}