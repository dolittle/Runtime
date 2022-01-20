// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ResourceTypes.Configuration.Specs.given;
using Machine.Specifications;

namespace Dolittle.Runtime.ResourceTypes.Configuration.Specs.for_TenantResourceManager;

public class when_getting_configuration_when_configuration_cannot_be_found_in_a_resource_representation : given.a_resource_type_with_one_service_and_a_system_providing_its_configuration
{
    static TenantResourceManager tenant_resource_manager;
    static Exception exception_result;
    Establish context = () => tenant_resource_manager = new TenantResourceManager(instance_of_mongo_db_read_models_representation_mock.Object, a_system_provding_resource_configuration_for_read_models_mock.Object);

    Because of_trying_to_get_the_configuration_for_the_second_resource_type = () => exception_result = Catch.Exception(() => tenant_resource_manager.GetConfigurationFor<configuration_for_second_resource_type>(tenant_id));

    It should_throw_an_exception = () => exception_result.ShouldNotBeNull();
    It should_throw_no_resource_type_matching_configuration_type = () => exception_result.ShouldBeOfExactType(typeof(NoResourceTypeMatchingConfigurationType));
}