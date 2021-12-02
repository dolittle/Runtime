// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Immutability.for_ImmutableExtensions;

public class when_checking_for_immutability_with_class_with_properties_without_setters
{
    static bool result;

    Because of = () => result = typeof(class_with_properties_without_setters).IsImmutable();

    It should_be_considered_immutable = () => result.ShouldBeTrue();
}