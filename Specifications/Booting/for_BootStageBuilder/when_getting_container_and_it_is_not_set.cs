// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Booting.for_BootStageBuilder;

public class when_getting_container_and_it_is_not_set : given.an_empty_boot_stage_builder
{
    static Exception result;

    Because of = () => result = Catch.Exception(() => builder.Container);

    It should_throw_container_not_set_yet = () => result.ShouldBeOfExactType<ContainerNotSetYet>();
}