// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IEvent">Event</see> in an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see>
    /// has not implemented the correct <see cref="IAmNextGenerationOf{T}">interface</see>.
    /// </summary>
    public class NotAMigratedEventType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotAMigratedEventType"/> class.
        /// </summary>
        /// <param name="type">Type of event.</param>
        public NotAMigratedEventType(Type type)
            : base($"The type '{type.AssemblyQualifiedName}' is not a valid migrated event type.  All events that are migrations of earlier generations of events" +
                    "must implement the IAmNextGenerationOf<T> interface where T is the previous generation of the event.")
        {
        }
    }
}