// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class test_mongodb_event_store : EventStore
    {
        readonly a_mongo_db_connection _database_runner;

        public test_mongodb_event_store(
            a_mongo_db_connection database_runner,
            Execution.IExecutionContextManager executionContextManager,
            EventStoreConnection connection,
            Events.IEventCommitter eventCommitter,
            Aggregates.IAggregateRoots aggregateRoots,
            Logging.ILogger logger)
            : base(executionContextManager, connection, eventCommitter, aggregateRoots, logger)
        {
            _database_runner = database_runner;
        }

        ~test_mongodb_event_store()
        {
            _database_runner.Dispose();
        }
    }
}