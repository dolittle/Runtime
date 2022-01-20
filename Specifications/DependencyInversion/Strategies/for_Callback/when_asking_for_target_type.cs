// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.DependencyInversion.Strategies.for_Callback;

public class when_asking_for_target_type
{
    static Func<object> underlying_callback;
    static Callback callback;
    static System.Type result;

    Establish context = () =>
    {
        underlying_callback = () => "Fourty Two";
        callback = new Callback(underlying_callback);
    };

    Because of = () => result = callback.GetTargetType();

    It should_return_the_type_of_the_target = () => result.ShouldEqual(underlying_callback.GetType());
}