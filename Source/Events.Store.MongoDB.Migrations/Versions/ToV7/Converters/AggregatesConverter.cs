// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using MongoDB.Driver;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.Versions.ToV7.Converters
{
    public class AggregatesConverter : IConvertFromOldToNew<Old.Aggregates.AggregateRoot, AggregateRoot>
    {
        public FilterDefinition<Old.Aggregates.AggregateRoot> Filter { get; } = Builders<Old.Aggregates.AggregateRoot>.Filter.Empty;

        public AggregateRoot Convert(Old.Aggregates.AggregateRoot old)
            => new(old.EventSource.ToString(), old.AggregateType, old.Version);
    }
}