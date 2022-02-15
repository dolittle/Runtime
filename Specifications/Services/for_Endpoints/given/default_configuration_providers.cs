// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Services.for_Endpoints.given;

public class default_configuration_providers
{
    protected static IEnumerable<ICanProvideDefaultConfigurationFor<EndpointsConfiguration>> default_providers;

    private Establish context = () =>
    {
        var mock = new Mock<IEnumerable<ICanProvideDefaultConfigurationFor<EndpointsConfiguration>>>();
        var list = new List<ICanProvideDefaultConfigurationFor<EndpointsConfiguration>>();

        mock.Setup(_ => _.GetEnumerator())
            .Returns(list.GetEnumerator());

        default_providers = mock.Object;
    };
}