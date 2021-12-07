// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Immutability.for_ImmutableExtensions.when_asserting_type;

public class should_be_immutable_with_record_with_setters
{
    static Exception exception;
    Because of = () => exception = Catch.Exception(() => typeof(record_with_setters).ShouldBeImmutable());

    It should_throw_writeable_immutable_properties_found = () => exception.ShouldBeOfExactType<WriteableImmutablePropertiesFound>();

}