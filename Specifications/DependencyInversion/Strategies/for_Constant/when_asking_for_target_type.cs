// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.Strategies.for_Constant
{
    public class when_asking_for_target_type
    {
        static Constant constant;
        static System.Type result;

        Establish context = () => constant = new Constant("Fourty Two");

        Because of = () => result = constant.GetTargetType();

        It should_return_the_type_of_the_target = () => result.ShouldEqual(typeof(string));
    }
}