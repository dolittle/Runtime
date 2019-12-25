// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Represents an exceptional situation where an <see cref="IEvent">Event</see> in an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see>
    /// has not implemented the correct <see cref="Dolittle.Runtime.Events.Migration.IAmNextGenerationOf{T}">interface</see>.
    /// </summary>
    public class NotAMigratedEventTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotAMigratedEventTypeException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public NotAMigratedEventTypeException(string message)
            : base(message)
        {
        }
    }
}