// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Logging;
using Grpc.Core;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_BoundServices
{
    public class when_checking_if_there_are_services_for_known_service_type
    {
        const string service_type = "My Service Type";
        static BoundServices bound_services;
        static bool result;

        Establish context = () =>
        {
            bound_services = new BoundServices(Moq.Mock.Of<ILogger>());
            var service = new Service(null, ServerServiceDefinition.CreateBuilder().Build(), null);
            bound_services.Register(service_type, new[] { service });
        };

        Because of = () => result = bound_services.HasFor(service_type);

        It should_return_true = () => result.ShouldBeTrue();
    }
}