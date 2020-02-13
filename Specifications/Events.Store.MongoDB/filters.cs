// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class filters
    {
        public static FilterDefinitionBuilder<Event> an_event_filter => Builders<Event>.Filter;

        public static FilterDefinitionBuilder<AggregateRoot> an_aggregate_filter => Builders<AggregateRoot>.Filter;
    }
}