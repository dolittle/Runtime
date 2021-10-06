// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB;
using Machine.Specifications;

namespace Dolittle.Runtime.Migrations.for_MigrationPerformer.given
{
    public class all_dependencies
    {
        protected static EventStoreConfiguration event_store_configuration;
        protected static resources_builder resources_builder;
        Establish context = () =>
        {
            event_store_configuration = new EventStoreConfiguration();
            resources_builder = new resources_builder();
        };
    }
}
