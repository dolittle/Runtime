// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Booting.for_BootStageBuilder
{
    public class when_getting_association_that_does_not_exist : given.initial_associations
    {
        const string third_key = "ThirdKey";
        static object third_value = "ThirdValue";

        static Exception result;

        Establish context = () =>
        {
            builder.Associate(third_key, third_value);
        };

        Because of = () => result = Catch.Exception(() => builder.GetAssociation("FourthKey"));

        It should_throw_missing_association = () => result.ShouldBeOfExactType<MissingAssociation>();
    }
}