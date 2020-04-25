// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventCommitter.given
{
    public class all_dependencies
    {
        protected const bool is_public = false;
        protected static an_event_store_connection an_event_store_connection;
        protected static Execution.ExecutionContext execution_context;
        protected static UncommittedEvent uncommitted_event;
        protected static EventSourceId event_source;

        Establish context = () =>
        {
            event_source = Guid.NewGuid();
            an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
            execution_context = execution_contexts.create();
            uncommitted_event = new UncommittedEvent(event_source, new Artifacts.Artifact(Guid.NewGuid(), 0), is_public, events.some_event_content);
        };

        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}