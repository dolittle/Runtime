// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;
namespace Dolittle.Runtime.Migrations.for_MigrationPerformer.when_performing_for_tenant
{
    public class and_event_store_is_not_configured_for_tenant : given.all_dependencies
    {
        static MigrationPerformer performer;
        static TenantId tenant;
        static Try result;
        
        Establish context = () =>
        {
            tenant = "b19d9c7d-1433-4b25-a34a-8d6dc522835f";
            performer = create_performer(_ => _.configure_tenant(tenant, _ => _.with_embeddings_store()));
        };

        Because of = () => result = performer.PerformForTenant(migration.Object, tenant).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_fail_because_event_store_is_not_configured_for_tenant = () => result.Exception.ShouldBeOfExactType<MissingResourceConfigurationForResourceTypeForTenant>();
    }
}