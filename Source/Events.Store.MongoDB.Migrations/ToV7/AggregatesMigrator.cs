// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="AggregatesMigrator"/>.
    /// </summary>
    public class AggregatesMigrator : BaseMigrator<Old.Aggregates.AggregateRoot, AggregateRoot>
    {
        public AggregatesMigrator(ICollectionNames collectionNames, IMongoCollectionMigrator migrator)
            : base(collectionNames, migrator)
        {
        }

        protected override IEnumerable<string> GetCollections(ICollectionNames collectionNames)
            => collectionNames.Aggregates;

        protected override AggregateRoot Convert(Old.Aggregates.AggregateRoot old)
            => new(old.EventSource.ToString(), old.AggregateType, old.Version);
    }
}