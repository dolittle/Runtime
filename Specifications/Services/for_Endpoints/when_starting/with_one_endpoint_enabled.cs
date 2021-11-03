// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_Endpoints.when_starting
{
    public class with_one_endpoint_enabled : given.one_service_type_with_binder
    {
        static EndpointConfiguration configuration = new();
        static Endpoints endpoints;

        Establish context = () =>
        {
            configuration = configuration with { Enabled = true };
            var endpoints_configuration = CreateEndpointsConfiguration(configuration);
            endpoints = new Endpoints(service_types, endpoints_configuration, default_providers, type_finder.Object, container.Object, bound_services.Object, logger);
        };

        Because of = () => endpoints.Start();

        It should_bind_services = () => binder.Verify(_ => _.BindServices(), Moq.Times.Once);
        It should_pass_services_to_bound_services = () => bound_services.Verify(_ => _.Register(service_type_identifier, Moq.It.IsAny<IEnumerable<Service>>()), Moq.Times.Once);
        It should_start_endpoint = () => endpoint.Verify(_ => _.Start(EndpointVisibility.Public, configuration, Moq.It.IsAny<IEnumerable<Service>>()), Moq.Times.Once);
    }
}