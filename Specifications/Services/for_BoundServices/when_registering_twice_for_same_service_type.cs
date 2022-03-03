// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac.Core;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_BoundServices;

public class when_registering_twice_for_same_service_type
{
    const string service_type = "My Service Type";

    static BoundServices bound_services;
    static Service first_service_first_type;
    static Service second_service_first_type;
    static Service first_service_second_type;
    static Service second_service_second_type;

    Establish context = () =>
    {
        bound_services = new BoundServices(Moq.Mock.Of<ILogger>());

        first_service_first_type = new Service(null, ServerServiceDefinition.CreateBuilder().Build(), null);
        second_service_first_type = new Service(null, ServerServiceDefinition.CreateBuilder().Build(), null);

        first_service_second_type = new Service(null, ServerServiceDefinition.CreateBuilder().Build(), null);
        second_service_second_type = new Service(null, ServerServiceDefinition.CreateBuilder().Build(), null);
    };

    Because of = () =>
    {
        bound_services.Register(service_type, new[] { first_service_first_type, second_service_first_type });
        bound_services.Register(service_type, new[] { first_service_second_type, second_service_second_type });
    };

    It should_hold_all_the_registered_services = () => bound_services.GetFor(service_type).ShouldContainOnly(new[]
    {
        first_service_first_type,
        second_service_first_type,
        first_service_second_type,
        second_service_second_type
    });
}