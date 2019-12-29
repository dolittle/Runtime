// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Exception that gets thrown when a <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see> is
    /// asked for a concrete type at a level that does not exist.
    /// This could be a level less than 0, or a level greater than the hierarchy depth.
    /// </summary>
    public class MissingMigrationLevel : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingMigrationLevel"/> class.
        /// </summary>
        /// <param name="logicalEvent">Type of event.</param>
        /// <param name="level">Generation level asked for.</param>
        public MissingMigrationLevel(Type logicalEvent, Generation level)
            : base($"Generation '{level}' is missing for '{logicalEvent.AssemblyQualifiedName}'")
        {
        }
    }
}