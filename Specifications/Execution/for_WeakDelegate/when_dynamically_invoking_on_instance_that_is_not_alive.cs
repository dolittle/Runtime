﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Execution.for_WeakDelegate;

public class when_dynamically_invoking_on_instance_that_is_not_alive
{
    static WeakDelegate setup()
    {
        var target = new ClassWithMethod();
        target = new ClassWithMethod();
        Func<string, double, int> @delegate = target.SomeMethod;
        var weakDelegate = new WeakDelegate(@delegate);
        return weakDelegate;
    }

    static WeakDelegate weak_delegate;
    static Exception exception;

    Establish context = () =>
    {
        weak_delegate = setup();
        GC.Collect(0, GCCollectionMode.Forced, true);
        GC.WaitForPendingFinalizers();
    };

    Because of = () => exception = Catch.Exception(() => weak_delegate.DynamicInvoke("Something", 42.42));

    It should_throw_target_not_alive = () => exception.ShouldBeOfExactType<CannotInvokeMethodBecauseTargetIsNotAlive>();
}