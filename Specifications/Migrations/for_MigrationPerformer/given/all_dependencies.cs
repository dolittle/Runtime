// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.Serialization.Json;
using Dolittle.Runtime.Types;
using Dolittle.Runtime.Types.Testing;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Migrations.for_MigrationPerformer.given
{
    public class all_dependencies
    {
        static ISerializer serializer;
        static IInstancesOf<ICanProvideConverters> converterProviders;
        protected static Mock<ICanMigrateAnEventStore> migration;

        Establish context = () =>
        {
            converterProviders = new StaticInstancesOf<ICanProvideConverters>();
            serializer = new Serializer(converterProviders);
            migration = new Mock<ICanMigrateAnEventStore>(MockBehavior.Strict);
        };

        protected static MigrationPerformer create_performer(Action<resources_builder> configureBuilder)
        {
            var builder = new resources_builder();
            configureBuilder(builder);

            var resources = builder.build();

            return new MigrationPerformer(
                new ResourceConfigurationsByTenantProvider(resources, serializer),
                resources);

        }
    }
}
