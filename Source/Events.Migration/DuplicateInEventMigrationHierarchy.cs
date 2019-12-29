// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see>
    /// has more than one migration path.
    /// </summary>
    public class DuplicateInEventMigrationHierarchy : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateInEventMigrationHierarchy"/> class.
        /// </summary>
        /// <param name="type">The type of event that is a duplicate in hierarchy.</param>
        /// <param name="logicalEvent">The logical event the hierarchy is for.</param>
        public DuplicateInEventMigrationHierarchy(Type type, Type logicalEvent)
            : base($"Type {type} already exists in the hierarchy for Event {logicalEvent}. Cannot have more than one migration path for an Event")
        {
        }
    }
}