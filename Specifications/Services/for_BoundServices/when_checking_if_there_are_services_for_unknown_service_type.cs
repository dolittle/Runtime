// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_BoundServices
{
    public class when_checking_if_there_are_services_for_unknown_service_type
    {
        const string service_type = "My Service Type";
        static BoundServices bound_services;
        static bool result;
        Establish context = () => bound_services = new BoundServices(Moq.Mock.Of<ILogger>());

        Because of = () => result = bound_services.HasFor(service_type);

        It should_return_false = () => result.ShouldBeFalse();
    }
}