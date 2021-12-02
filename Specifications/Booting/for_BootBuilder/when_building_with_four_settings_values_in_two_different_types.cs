// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Booting.for_BootBuilder;

public class when_building_with_four_settings_values_in_two_different_types
{
    class first_settings_type : IRepresentSettingsForBootStage
    {
        public string first_property { get; set; }

        public string second_property { get; set; }
    }

    class second_settings_type : IRepresentSettingsForBootStage
    {
        public string first_property { get; set; }

        public string second_property { get; set; }
    }

    const string first = "First";
    const string second = "Second";
    const string third = "third";
    const string fourth = "fourth";

    static BootBuilder builder;
    static Boot result;

    Establish context = () =>
    {
        builder = new BootBuilder();
        builder.Set<first_settings_type>(_ => _.first_property, first);
        builder.Set<first_settings_type>(_ => _.second_property, second);
        builder.Set<second_settings_type>(_ => _.first_property, third);
        builder.Set<second_settings_type>(_ => _.second_property, fourth);
    };

    Because of = () => result = builder.Build();

    It should_hold_first_value_in_first_settings = () => ((first_settings_type)result.GetSettingsByType(typeof(first_settings_type))).first_property.ShouldEqual(first);
    It should_hold_second_value_in_first_settings = () => ((first_settings_type)result.GetSettingsByType(typeof(first_settings_type))).second_property.ShouldEqual(second);
    It should_hold_third_value_in_second_settings = () => ((second_settings_type)result.GetSettingsByType(typeof(second_settings_type))).first_property.ShouldEqual(third);
    It should_hold_fourth_value_in_second_settings = () => ((second_settings_type)result.GetSettingsByType(typeof(second_settings_type))).second_property.ShouldEqual(fourth);

    It should_return_same_object_instance_for_type_twice = () =>
    {
        var first = result.GetSettingsByType(typeof(second_settings_type));
        var second = result.GetSettingsByType(typeof(second_settings_type));
        first.GetHashCode().ShouldEqual(second.GetHashCode());
    };
}