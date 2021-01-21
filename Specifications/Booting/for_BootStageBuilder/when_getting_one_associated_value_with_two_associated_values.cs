// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Booting.for_BootStageBuilder
{
    public class when_getting_one_associated_value_with_two_associated_values : given.an_empty_boot_stage_builder
    {
        const string first_key = "FirstKey";
        const string second_key = "SecondKey";
        static object first_value = "FirstValue";

        static object second_value = "SecondValue";

        static object result;

        Establish context = () =>
        {
            builder.Associate(first_key, first_value);
            builder.Associate(second_key, second_value);
        };

        Because of = () => result = builder.GetAssociation(second_key);

        It should_return_the_associated_value = () => result.ShouldEqual(second_value);
    }
}