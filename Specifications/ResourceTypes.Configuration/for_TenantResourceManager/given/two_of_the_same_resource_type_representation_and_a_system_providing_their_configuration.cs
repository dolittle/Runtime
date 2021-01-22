// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.ResourceTypes.Configuration.Specs.given;
using Dolittle.Runtime.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.ResourceTypes.Configuration.Specs.for_TenantResourceManager.given
{
    public class two_of_the_same_resource_type_representation_and_a_system_providing_their_configuration : all_dependencies
    {
        protected static Mock<IInstancesOf<IRepresentAResourceType>> instance_of_mongo_db_read_models_representation_mock;
        protected static Mock<ICanProvideResourceConfigurationsByTenant> a_system_provding_resource_configuration_for_read_models_mock;

        Establish context = () =>
        {
            var first_read_models_representation = new resource_type_with_first_service_for_first_resource_type_and_first_implementation();
            var second_read_models_representation = new resource_type_with_first_service_for_first_resource_type_and_first_implementation();
            var read_models_representations = new List<IRepresentAResourceType> { first_read_models_representation, second_read_models_representation };
            instance_of_mongo_db_read_models_representation_mock = new Mock<IInstancesOf<IRepresentAResourceType>>();
            instance_of_mongo_db_read_models_representation_mock.Setup(_ => _.GetEnumerator()).Returns(read_models_representations.GetEnumerator());

            a_system_provding_resource_configuration_for_read_models_mock = new Mock<ICanProvideResourceConfigurationsByTenant>();
            a_system_provding_resource_configuration_for_read_models_mock.Setup(_ => _.ConfigurationFor<configuration_for_first_resource_type>(tenant_id, first_resource_type)).Returns(new configuration_for_first_resource_type());
        };
    }
}