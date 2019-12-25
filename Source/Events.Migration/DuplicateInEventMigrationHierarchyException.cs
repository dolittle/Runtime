// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Represents an exceptional situation where an event in an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see>
    /// has more than one migration path.
    /// </summary>
    public class DuplicateInEventMigrationHierarchyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateInEventMigrationHierarchyException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DuplicateInEventMigrationHierarchyException(string message)
            : base(message)
        {
        }
    }
}