// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Machine.Specifications;

namespace Dolittle.Runtime.Reflection.Specs.for_TypeExtensions;

public class when_getting_default_constructor_from_type_with_default_constructor
{
    static ConstructorInfo constructor_info;

    Because of = () => constructor_info = typeof(TypeWithDefaultConstructor).GetDefaultConstructor();

    It should_return_a_constructor = () => constructor_info.ShouldNotBeNull();
}