// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.Autofac.Tenancy.for_InstancesPerTenant.given
{
    public class no_instances : all_dependencies
    {
        protected static InstancesPerTenant instances_per_tenant;

        Establish context = () => instances_per_tenant = new InstancesPerTenant(tenant_key_creator.Object, type_activator.Object);
    }
}