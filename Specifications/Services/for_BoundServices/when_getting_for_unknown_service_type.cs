// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using Machine.Specifications;

namespace Dolittle.Runtime.Services.for_BoundServices;

public class when_getting_for_unknown_service_type
{
    const string service_type = "My Service Type";
    static BoundServices bound_services;
    static Exception result;

    Establish context = () => bound_services = new BoundServices(Moq.Mock.Of<ILogger>());

    Because of = () => result = Catch.Exception(() => bound_services.GetFor(service_type));

    It should_throw_unknown_service_type = () => result.ShouldBeOfExactType<UnknownServiceType>();
}