// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolittle.DependencyInversion;
using Dolittle.Events;
using Dolittle.Lifecycle;
using Dolittle.Types;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Represents a <see cref="IEventMigratorManager">IEventMigratorManager</see>.
    /// </summary>
    /// <remarks>
    /// The manager will automatically import any <see cref="IEventMigrator{TS,TD}">IEventMigrator</see>
    /// and use them when migrating.
    /// </remarks>
    [Singleton]
    public class EventMigratorManager : IEventMigratorManager
    {
        readonly ITypeFinder _typeFinder;
        readonly IContainer _container;
        readonly Dictionary<Type, Type> _migratorTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMigratorManager"/> class.
        /// </summary>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> to use for discovering <see cref="IEventMigrator{T1,T2}">Event migrators</see>.</param>
        /// <param name="container"><see cref="IContainer"/> to use for instantiation of <see cref="IEventMigrator{T1,T2}">Event migrators</see>.</param>
        public EventMigratorManager(ITypeFinder typeFinder, IContainer container)
        {
            _typeFinder = typeFinder;
            _container = container;
            _migratorTypes = new Dictionary<Type, Type>();
            Initialize();
        }

        /// <inheritdoc/>
        public IEvent Migrate(IEvent @event)
        {
            var result = @event;
            while (_migratorTypes.TryGetValue(result.GetType(), out Type migratorType))
            {
                var migrator = (dynamic)_container.Get(migratorType);
                result = (IEvent)migrator.Migrate((dynamic)result);
            }

            return result;
        }

        /// <summary>
        /// Register a migrator.
        /// </summary>
        /// <param name="migratorType">Type of migrator to register.</param>
        public void RegisterMigrator(Type migratorType)
        {
            // Todo : Validate migrator type!
            var sourceType = GetSourceType(migratorType);
            _migratorTypes.Add(sourceType, migratorType);
        }

        Type GetSourceType(Type migratorType)
        {
            var @interface = migratorType.GetTypeInfo().ImplementedInterfaces.First(i =>
                i.GetTypeInfo().IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IEventMigrator<,>));

            return @interface.GetTypeInfo().GenericTypeArguments[0];
        }

        void Initialize()
        {
            var migratorTypes = _typeFinder.FindMultiple(typeof(IEventMigrator<,>));
            foreach (var migrator in migratorTypes)
            {
                RegisterMigrator(migrator);
            }
        }
    }
}