// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Immutability.for_ImmutableExtensions.when_asserting_type
{
    public class should_be_immutable_with_class_with_readonly_fields
    {
        static Exception exception;
        Because of = () => exception = Catch.Exception(() => typeof(class_with_only_readonly_fields).ShouldBeImmutable());

        It should_not_throw_an_exception = () => exception.ShouldBeNull();
    }
}
