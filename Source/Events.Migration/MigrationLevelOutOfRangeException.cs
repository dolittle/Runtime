// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Represents an exception situation where a <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see> is
    /// asked for a concrete type at a level that does not exist.
    /// This could be a level less than 0, or a level greater than the hierarchy depth.
    /// </summary>
    public class MigrationLevelOutOfRangeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationLevelOutOfRangeException"/> class.
        /// </summary>
        /// <param name="message">Error Message.</param>
        public MigrationLevelOutOfRangeException(string message)
            : base(message)
        {
        }
    }
}