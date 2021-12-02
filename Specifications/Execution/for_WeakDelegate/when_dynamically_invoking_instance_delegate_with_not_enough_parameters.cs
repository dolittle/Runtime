﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Execution.for_WeakDelegate;

public class when_dynamically_invoking_instance_delegate_with_not_enough_parameters
{
    static ClassWithMethod target;
    static WeakDelegate weak_delegate;
    static Exception exception;

    Establish context = () =>
    {
        target = new ClassWithMethod();
        Func<string, double, int> @delegate = target.SomeMethod;
        weak_delegate = new WeakDelegate(@delegate);
    };

    Because of = () => exception = Catch.Exception(() => weak_delegate.DynamicInvoke(43));

    It should_throw_invalid_signature_exception = () => exception.ShouldBeOfExactType<InvalidMethodSignature>();
}