// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Machine.Specifications;

namespace Dolittle.Runtime.Booting.for_BootStageBuilder;

public class when_getting_container_and_it_is_set : given.an_empty_boot_stage_builder
{
    static IContainer result;
    static IContainer expected;

    Establish context = () =>
    {
        expected = Moq.Mock.Of<IContainer>();
        builder.UseContainer(expected);
    };

    Because of = () => result = builder.Container;

    It should_return_the_used_container = () => result.ShouldEqual(expected);
}