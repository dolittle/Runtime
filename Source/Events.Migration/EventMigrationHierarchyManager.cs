// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Lifecycle;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Represents a <see cref="IEventMigrationHierarchyManager">IEventMigrationHierarchyManager</see>.
    /// </summary>
    /// <remarks>
    /// The manager will automatically build an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see> for all events and
    /// allow clients to query for the current migration level for a specific logical event or the concrete type of a particular link
    /// in the migration chain for a logical event.
    /// </remarks>
    [Singleton]
    public class EventMigrationHierarchyManager : IEventMigrationHierarchyManager
    {
        readonly IEventMigrationHierarchyDiscoverer _eventMigrationHierarchyDiscoverer;
        readonly IEnumerable<EventMigrationHierarchy> _hierarchies;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMigrationHierarchyManager"/> class.
        /// </summary>
        /// <param name="eventMigrationHierarchyDiscoverer">IEventMigrationHierarchyDiscoverer.</param>
        public EventMigrationHierarchyManager(IEventMigrationHierarchyDiscoverer eventMigrationHierarchyDiscoverer)
        {
            _eventMigrationHierarchyDiscoverer = eventMigrationHierarchyDiscoverer;
            _hierarchies = _eventMigrationHierarchyDiscoverer.GetMigrationHierarchies();
        }

        /// <inheritdoc/>
        public Generation GetCurrentGenerationFor(Type logicalEvent)
        {
            var hierarchy = GetHierarchyForLogicalType(logicalEvent);
            return (uint)hierarchy.MigrationLevel;
        }

        /// <inheritdoc/>
        public Type GetTargetTypeForGeneration(Type logicalEvent, Generation level)
        {
            if (level < 0)
                throw new MissingMigrationLevel(logicalEvent, level);

            var hierarchy = GetHierarchyForLogicalType(logicalEvent);
            Type type;
            try
            {
                type = hierarchy.GetConcreteTypeForLevel((int)level.Value);
            }
            catch (Exception)
            {
                throw new MissingMigrationLevel(logicalEvent, level);
            }

            return type;
        }

        /// <inheritdoc/>
        public Type GetLogicalTypeFor(Type @event)
        {
            var hierarchy = _hierarchies.FirstOrDefault(h => h.MigratedTypes.Contains(@event));

            if (hierarchy == null)
                throw new MissingEventTypeInHierarchy(@event.AssemblyQualifiedName);

            return hierarchy.LogicalEvent;
        }

        /// <inheritdoc/>
        public Type GetLogicalTypeFromName(string typeName)
        {
            var hierarchy = _hierarchies.FirstOrDefault(h => h.LogicalEvent.Name == typeName);

            if (hierarchy == null)
                throw new MissingEventTypeInHierarchy(typeName);

            return hierarchy.LogicalEvent;
        }

        EventMigrationHierarchy GetHierarchyForLogicalType(Type logicalEvent)
        {
            var hierarchy = _hierarchies.FirstOrDefault(hal => hal.LogicalEvent == logicalEvent);

            if (hierarchy == null)
                throw new MissingEventTypeInHierarchy(logicalEvent.AssemblyQualifiedName);

            return hierarchy;
        }
    }
}