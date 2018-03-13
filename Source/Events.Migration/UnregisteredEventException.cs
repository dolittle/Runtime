/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
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
        /// Initializes a <see cref="UnregisteredEventException"/>
        /// </summary>
        /// <param name="message">Error Message</param>
        public UnregisteredEventException(string message) : base(message)
        {}
    }
}