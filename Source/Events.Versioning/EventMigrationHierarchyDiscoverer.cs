/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doLittle.Execution;
using doLittle.Types;

namespace doLittle.Events
{
    /// <summary>
    /// Represents a <see cref="IEventMigrationHierarchyDiscoverer">IEventMigrationHierarchyDiscoverer</see>
    /// </summary>
    /// <remarks>
    /// The discoverer will automatically build an <see cref="EventMigrationHierarchy">EventMigrationHierarchy</see> for all events.
    /// </remarks>
    [Singleton]
    public class EventMigrationHierarchyDiscoverer : IEventMigrationHierarchyDiscoverer
    {
        private readonly ITypeFinder _typeFinder;
        private static readonly Type _migrationInterface = typeof (IAmNextGenerationOf<>);

        /// <summary>
        /// Initializes an instance of <see cref="EventMigrationHierarchyDiscoverer"/>
        /// </summary>
        /// <param name="typeFinder"></param>
        public EventMigrationHierarchyDiscoverer(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        /// <inheritdoc/>
        public IEnumerable<EventMigrationHierarchy> GetMigrationHierarchies()
        {
            var allEvents = GetAllEventTypes();
            var logicalEvents = GetLogicalEvents(allEvents);
            return logicalEvents.Select(logicalEvent => GetMigrationHierarchy(logicalEvent,allEvents)).ToList();
        }


        static EventMigrationHierarchy GetMigrationHierarchy(Type logicalEvent, IEnumerable<Type> allEvents)
        {
            var migrationHierarchy = new EventMigrationHierarchy(logicalEvent);
            var migratedEvents = GetMigratedEvents(allEvents);

            var migrationType = GetMigrationTypeFor(logicalEvent, migratedEvents);
            while (migrationType != null)
            {
                migrationHierarchy.AddMigrationLevel(migrationType);
                migrationType = GetMigrationTypeFor(migrationType, migratedEvents);
            }
            return migrationHierarchy;
        }

        static Type GetMigrationTypeFor(Type migrationSourceType, IEnumerable<Type> migratedEventTypes)
        {
            return migratedEventTypes.Select(candidateType => GetMigrationType(migrationSourceType, candidateType)).FirstOrDefault(type => type != null);
        }

        static Type GetMigrationType(Type migrationSourceType, Type candidateType)
        {
            var types = from interfaceType in 
                            candidateType.GetTypeInfo().ImplementedInterfaces
                        where interfaceType.GetTypeInfo().IsGenericType

                        let baseInterface = interfaceType.GetGenericTypeDefinition()
                        where baseInterface == _migrationInterface && interfaceType
                            .GetTypeInfo().GenericTypeArguments
                            .First() == migrationSourceType
                        select interfaceType
                            .GetTypeInfo().GenericTypeArguments
                            .First();

            var migratedFromType = types.FirstOrDefault();

            return migratedFromType == null ? null : candidateType;
        }

        static IEnumerable<Type> GetLogicalEvents(IEnumerable<Type> allEventTypes)
        {
            var migratedEvents = GetMigratedEvents(allEventTypes);

            return allEventTypes.Except(migratedEvents);
        }

        static IEnumerable<Type> GetMigratedEvents(IEnumerable<Type> allEventTypes)
        {
            foreach(var @event in allEventTypes)
            {
                var eventType = (from ievent in @event
                                    .GetTypeInfo().ImplementedInterfaces
                                 where ievent
                                    .GetTypeInfo().IsGenericType
                                 let baseInterface = ievent.GetGenericTypeDefinition()
                                 where baseInterface == _migrationInterface
                                 select ievent).FirstOrDefault();

                if (eventType != null)
                    yield return @event;
            }
        }

        IEnumerable<Type> GetAllEventTypes()
        {
            return _typeFinder.FindMultiple<IEvent>();
        }
    }
}