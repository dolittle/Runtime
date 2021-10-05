// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Defines a system that knows about migrations of data stores between two versions of the Runtime.
    /// </summary>
    public interface ICanMigrateDataStores
    {
        /// <summary>
        /// Checks whether the migration can migrate data stores between the provided versions.
        /// </summary>
        /// <param name="from">The version to migrate from.</param>
        /// <param name="to">The version to migrate to.</param>
        /// <returns>True if it can migrate between the provided versions, false if not.</returns>
        bool CanMigrateFor(Version from, Version to);

        /// <summary>
        /// Gets the migrator for the Event Store.
        /// </summary>
        ICanMigrateAnEventStore EventStore { get; }
    }
}
