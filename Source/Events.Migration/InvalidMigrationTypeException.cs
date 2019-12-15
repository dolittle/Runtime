// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Represents an exceptional situation where an <see cref="IEvent">Event</see> in an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see>
    /// has does not migrate from the previous event in the migration hierarchy.
    /// </summary>
    public class InvalidMigrationTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMigrationTypeException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public InvalidMigrationTypeException(string message)
            : base(message)
        {
        }
    }
}