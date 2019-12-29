// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Exception that gets thrown when an <see cref="IEvent">Event</see> in an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see>
    /// has does not migrate from the previous event in the migration hierarchy.
    /// </summary>
    public class InvalidMigrationType : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidMigrationType"/> class.
        /// </summary>
        /// <param name="expectedType">The expected migration type.</param>
        /// <param name="actualType">The actual migration type.</param>
        public InvalidMigrationType(Type expectedType, Type actualType)
            : base($"Expected migration for type {expectedType} but got migration for type {actualType} instead.")
        {
        }
    }
}