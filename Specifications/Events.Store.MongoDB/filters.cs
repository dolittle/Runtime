// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Aggregates;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class filters
    {
        public static FilterDefinitionBuilder<Event> event_filter => Builders<Event>.Filter;

        public static FilterDefinitionBuilder<StreamEvent> stream_event_filter => Builders<StreamEvent>.Filter;

        public static FilterDefinitionBuilder<PublicEvent> public_event_filter => Builders<PublicEvent>.Filter;

        public static FilterDefinitionBuilder<AggregateRoot> an_aggregate_filter => Builders<AggregateRoot>.Filter;
    }
}