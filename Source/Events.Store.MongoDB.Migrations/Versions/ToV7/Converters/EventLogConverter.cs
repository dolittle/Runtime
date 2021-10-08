// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters
{
    public class EventLogConverter : IConvertFromOldToNew<Old.Events.Event, Events.Event>
    {
        public FilterDefinition<Old.Events.Event> Filter { get; } = Builders<Old.Events.Event>.Filter.Empty;

        public Events.Event Convert(Old.Events.Event old)
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