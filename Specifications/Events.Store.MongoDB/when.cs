// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class when
    {
        static a_mongo_db_connection runner;
        static an_event_store_connection connection;
        static Event @event = events.random_event_not_from_aggregate_with_position_and_event_log_version(0, 0);

        Establish context = () =>
        {
            runner = new a_mongo_db_connection();
            connection = new an_event_store_connection(runner);
        };

        Because of = () => connection.AllStream.InsertOne(@event);

        It should = () => connection.AllStream.Find(Builders<Event>.Filter.Empty).ToList().Count.ShouldEqual(1);
    }
}