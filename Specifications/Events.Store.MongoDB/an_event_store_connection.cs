// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Logging;
using Moq;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public class an_event_store_connection : EventStoreConnection
    {
        readonly a_mongo_db_connection _runner;

        public an_event_store_connection(a_mongo_db_connection runner)
            : base(runner.Connection, Mock.Of<ILogger>())
        {
            _runner = runner;
        }

        ~an_event_store_connection()
        {
            _runner.Dispose();
        }
    }
}