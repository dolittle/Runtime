// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dolittle.Runtime.Events.Migration
{
    /// <summary>
    /// Represents a migration hierarchy for a logical event, containing the concrete type for each step in the chain.
    /// </summary>
    public class EventMigrationHierarchy
    {
        readonly List<Type> _migrationLevels;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMigrationHierarchy"/> class.
        /// </summary>
        /// <param name="logicalEvent">Logical event that the hierarchy relates to.</param>
        public EventMigrationHierarchy(Type logicalEvent)
        {
            _migrationLevels = new List<Type>();
            LogicalEvent = logicalEvent;
            AddMigrationLevel(logicalEvent);
        }

        /// <summary>
        /// Gets the logical event type.
        /// </summary>
        public Type LogicalEvent { get; }

        /// <summary>
        /// Gets the migration level of the hierarchy.
        /// </summary>
        public int MigrationLevel
        {
            get { return _migrationLevels.Count - 1; }
        }

        /// <summary>
        /// Gets the types in the migration hierarchy.
        /// </summary>
        public IEnumerable<Type> MigratedTypes => _migrationLevels.ToArray();

        /// <summary>
        /// Adds a new concrete type as the next level in the migration hierarchy.
        /// </summary>
        /// <param name="type">Concrete type of the logical event.</param>
        public void AddMigrationLevel(Type type)
        {
            if (_migrationLevels.Contains(type))
            {
                throw new DuplicateInEventMigrationHierarchy(type, LogicalEvent);
            }

            if (MigrationLevel >= 0)
                ValidateMigration(type);

            _migrationLevels.Add(type);
        }

        /// <summary>
        /// Gets the concrete type of the logical event at the specified migration level.
        /// </summary>
        /// <param name="level">The migration level.</param>
        /// <returns>Concrete type of the logical event at the specified migration level.</returns>
        public Type GetConcreteTypeForLevel(int level)
        {
            return _migrationLevels[level];
        }

        /// <summary>
        /// Gets the level which the concrete type occupies in the migration hierarchy.
        /// </summary>
        /// <param name="type">Concrete type of the logical event.</param>
        /// <returns>The migration level.</returns>
        public int GetLevelForConcreteType(Type type)
        {
            return _migrationLevels.IndexOf(type);
        }

        void ValidateMigration(Type type)
        {
            ValidateTypeIsAMigration(type);
            ValidateTypeIsOfExpectedType(type);
        }

        void ValidateTypeIsAMigration(Type type)
        {
            if (!ImplementsMigrationInterface(type))
            {
                throw new NotAMigratedEventType(type);
            }
        }

        void ValidateTypeIsOfExpectedType(Type type)
        {
            var expectedTypeToMigrateFrom = _migrationLevels[MigrationLevel];
            try
            {
                var actualTypeMigratingFrom = GetMigrationFromType(type);

                if (actualTypeMigratingFrom != expectedTypeToMigrateFrom)
                {
                    ThrowInvalidMigrationTypeException(expectedTypeToMigrateFrom, type);
                }
            }
            catch
            {
                ThrowInvalidMigrationTypeException(expectedTypeToMigrateFrom, type);
            }
        }

        Type GetMigrationFromType(Type migrationType)
        {
            var @interface = migrationType.GetTypeInfo().ImplementedInterfaces.Last(i =>
                i.GetTypeInfo().IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IAmNextGenerationOf<>));

            return @interface.GetTypeInfo().GenericTypeArguments[0];
        }

        bool ImplementsMigrationInterface(Type migrationType)
        {
            return migrationType.GetTypeInfo().ImplementedInterfaces.Any(i =>
                i.GetTypeInfo().IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IAmNextGenerationOf<>));
        }

        void ThrowInvalidMigrationTypeException(Type expected, Type actual)
        {
            throw new InvalidMigrationType(expected, actual);
        }
    }
}