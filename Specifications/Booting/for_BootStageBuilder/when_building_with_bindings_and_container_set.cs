// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Strategies;
using Machine.Specifications;

namespace Dolittle.Runtime.Booting.for_BootStageBuilder;

public class when_building_with_bindings_and_container_set : given.an_empty_boot_stage_builder
{
    const string value_bound_to = "42 - Fourty Two";
    const string a_key = "Its a Key";
    static object a_value = "Its a Value";

    static IContainer container;
    static BootStageResult result;

    Establish context = () =>
    {
        container = Moq.Mock.Of<IContainer>();
        builder.UseContainer(container);
        builder.Bindings.Bind(typeof(object)).To(value_bound_to);
        builder.Associate(a_key, a_value);
    };

    Because of = () => result = builder.Build();

    It should_hold_the_container = () => result.Container.ShouldEqual(container);
    It should_hold_the_binding = () => result.Bindings.First().Service.ShouldEqual(typeof(object));
    It should_hold_the_binding_target = () => ((Constant)result.Bindings.First().Strategy).Target.ShouldEqual(value_bound_to);
    It should_hold_the_association = () => result.Associations[a_key].ShouldEqual(a_value);
}