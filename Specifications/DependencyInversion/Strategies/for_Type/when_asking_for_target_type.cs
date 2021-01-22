// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.Strategies.for_Type
{
    public class when_asking_for_target_type
    {
        static Type type;
        static System.Type result;

        Establish context = () =>
        {
            type = new Type(typeof(string));
        };

        Because of = () => result = type.GetTargetType();

        It should_return_the_type_of_the_target = () => result.ShouldEqual(typeof(string));
    }
}