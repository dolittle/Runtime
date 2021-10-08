// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters
{
    public class StreamConverter : IConvertFromOldToNew<Old.Events.StreamEvent, Events.StreamEvent>
    {
        public FilterDefinition<Old.Events.StreamEvent> Filter { get; } = Builders<Old.Events.StreamEvent>.Filter.Empty;

        public Events.StreamEvent Convert(Old.Events.StreamEvent old)
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