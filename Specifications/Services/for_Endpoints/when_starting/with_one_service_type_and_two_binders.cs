// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_Endpoints.when_starting
{
    public class with_one_service_type_and_two_binders : given.one_service_type_with_two_binders
    {
        static Endpoints endpoints;

        Establish context = () =>
        {
            configuration.Enabled = true;
            endpoints = new Endpoints(service_types, endpoints_configuration, type_finder.Object, container.Object, bound_services.Object, logger);
        };

        Because of = () => endpoints.Start();
        It should_start_endpoint_once = () => endpoint.Verify(_ => _.Start(EndpointVisibility.Public, configuration, Moq.It.IsAny<IEnumerable<Service>>()), Moq.Times.Once);
    }
}