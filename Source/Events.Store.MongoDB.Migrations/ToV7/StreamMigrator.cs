// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7
{
    /// <summary>
    /// Represents an implementation of <see cref="EventLogMigrator"/>.
    /// </summary>
    public class StreamMigrator : BaseMigrator<Old.Events.StreamEvent, Events.StreamEvent>
    {
        public StreamMigrator(ICollectionNames collectionNames, IMongoCollectionMigrator migrator)
            : base(collectionNames, migrator)
        {
        }

        protected override IEnumerable<string> GetCollections(ICollectionNames collectionNames)
            => collectionNames.EventStreams;

        protected override Events.StreamEvent Convert(Old.Events.StreamEvent old)
            => new (
                old.StreamPosition,
                old.Partition.ToString(),
                old.ExecutionContext,
                new Events.StreamEventMetadata(
                    old.Metadata.EventLogSequenceNumber,
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