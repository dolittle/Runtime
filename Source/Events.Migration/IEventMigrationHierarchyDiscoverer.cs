// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Defines a discoverer that is able to detect all events and group them into <see cref="EventMigrationHierarchy">EventMigrationHierarchies</see>.
    /// </summary>
    public interface IEventMigrationHierarchyDiscoverer
    {
        /// <summary>
        /// Gets all the <see cref="EventMigrationHierarchy">EventMigrationHierarchies</see> for events.
        /// </summary>
        /// <returns>Event Migration Hierarchies.</returns>
        IEnumerable<EventMigrationHierarchy> GetMigrationHierarchies();
    }
}