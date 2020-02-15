// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventTypesFromStreamsFetcher.given
{
    public class all_dependencies
    {
        protected static an_event_store_connection an_event_store_connection;

        Establish context = () => an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}