// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ResourceTypes.Configuration.Specs.given;
using Machine.Specifications;

namespace Dolittle.Runtime.ResourceTypes.Configuration.Specs.for_TenantResourceManager
{
    public class when_getting_configuration_when_configuration_is_found_in_two_resource_representations : given.two_of_the_same_resource_type_representation_and_a_system_providing_their_configuration
    {
        static TenantResourceManager tenant_resource_manager;
        static Exception exception_result;
        Establish context = () => tenant_resource_manager = new TenantResourceManager(instance_of_mongo_db_read_models_representation_mock.Object, a_system_provding_resource_configuration_for_read_models_mock.Object);

        Because of_trying_to_get_a_read_model_repository_configuration = () => exception_result = Catch.Exception(() => tenant_resource_manager.GetConfigurationFor<configuration_for_first_resource_type>(tenant_id));

        It should_throw_an_exception = () => exception_result.ShouldNotBeNull();
        It should_throw_configuration_type_mapped_to_multiple_resource_types = () => exception_result.ShouldBeOfExactType(typeof(ConfigurationTypeMappedToMultipleResourceTypes));
    }
}