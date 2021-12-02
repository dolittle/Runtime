// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB;

public static class filters
{
    public static FilterDefinitionBuilder<MongoDB.Events.Event> event_filter => Builders<MongoDB.Events.Event>.Filter;

    public static FilterDefinitionBuilder<MongoDB.Events.StreamEvent> stream_event_filter => Builders<MongoDB.Events.StreamEvent>.Filter;

    public static FilterDefinitionBuilder<MongoDB.Aggregates.AggregateRoot> an_aggregate_filter => Builders<MongoDB.Aggregates.AggregateRoot>.Filter;
}