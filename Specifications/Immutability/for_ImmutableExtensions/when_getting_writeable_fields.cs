// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Machine.Specifications;

namespace Dolittle.Runtime.Immutability.for_ImmutableExtensions;

public class when_getting_writeable_fields
{
    static FieldInfo[] fields;

    Because of = () => fields = typeof(class_with_readonly_fields).GetWriteableFields();

    It should_return_the_writeable_field = () => fields[0].Name.ShouldEqual("field_that_can_write");
}