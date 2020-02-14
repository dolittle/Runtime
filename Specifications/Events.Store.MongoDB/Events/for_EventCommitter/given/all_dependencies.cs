// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Dolittle.Execution;
using Dolittle.Security;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventCommitter.given
{
    public class all_dependencies
    {
        protected static an_event_store_connection an_event_store_connection;
        protected static ExecutionContext execution_context;
        protected static UncommittedEvent uncommitted_event;

        Establish context = () =>
        {
            an_event_store_connection = new an_event_store_connection(new a_mongo_db_connection());
            execution_context = new ExecutionContext(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "dev",
                Guid.NewGuid(),
                Claims.Empty,
                CultureInfo.InvariantCulture);
            uncommitted_event = new UncommittedEvent(new Artifacts.Artifact(Guid.NewGuid(), 0), events.some_event_content);
        };

        Cleanup cleanup = () => an_event_store_connection.Dispose();
    }
}