// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="EventLogMigrator"/>.
    /// </summary>
    public class EventLogMigrator : BaseMigrator<Old.Events.Event, Events.Event>
    {
        public EventLogMigrator(ICollectionNames collectionNames, IMongoCollectionMigrator migrator)
            : base(collectionNames, migrator)
        {
        }
        protected override IEnumerable<string> GetCollections(ICollectionNames collectionNames)
            => collectionNames.EventLogs;
        
        protected override Events.Event Convert(Old.Events.Event old)
            => new (
                old.EventLogSequenceNumber,
                old.ExecutionContext,
                new Events.EventMetadata(
                    old.Metadata.Occurred,
                    old.Metadata.EventSource.ToString(),
                    old.Metadata.TypeId,
                    old.Metadata.TypeGeneration,
                    old.Metadata.Public),
                old.Aggregate,
                old.EventHorizon,
                old.Content);
    }
}