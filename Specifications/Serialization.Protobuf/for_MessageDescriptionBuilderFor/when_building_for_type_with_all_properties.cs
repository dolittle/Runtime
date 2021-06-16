// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Serialization.Protobuf.for_MessageDescriptionBuilderFor
{
    public class when_building_for_type_with_all_properties
    {
        static MessageDescriptionBuilderFor<simple_object_with_primitives> builder;
        static MessageDescription result;

        Establish context = () => builder = new MessageDescriptionBuilderFor<simple_object_with_primitives>(typeof(simple_object_with_primitives).Name, Array.Empty<IPropertyDescriptionBuilder>());

        Because of = () => result = builder.WithAllProperties().Build();

        It should_build_a_message_description = () => result.ShouldNotBeNull();
        It should_include_guid_property = () => result.Properties.SingleOrDefault(_ => _.Property == simple_object_with_primitives.a_guid_property).ShouldNotBeNull();
        It should_include_integer_property = () => result.Properties.SingleOrDefault(_ => _.Property == simple_object_with_primitives.an_integer_property).ShouldNotBeNull();
        It should_include_float_property = () => result.Properties.SingleOrDefault(_ => _.Property == simple_object_with_primitives.a_float_property).ShouldNotBeNull();
        It should_include_double_property = () => result.Properties.SingleOrDefault(_ => _.Property == simple_object_with_primitives.a_double_property).ShouldNotBeNull();
        It should_include_string_property = () => result.Properties.SingleOrDefault(_ => _.Property == simple_object_with_primitives.a_string_property).ShouldNotBeNull();
        It should_include_date_time_property = () => result.Properties.SingleOrDefault(_ => _.Property == simple_object_with_primitives.a_date_time_property).ShouldNotBeNull();
        It should_include_date_time_offset_property = () => result.Properties.SingleOrDefault(_ => _.Property == simple_object_with_primitives.a_date_time_offset_property).ShouldNotBeNull();
    }
}