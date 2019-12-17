// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Migration.Specs.Fakes.v2;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationHierarchy
{
    public class when_getting_the_concrete_type_for_a_level : given.an_event_migration_hierarchy_with_two_levels
    {
        static Type level_one_type;
        static Type level_two_type;

        Because of = () =>
                         {
                             level_one_type = event_migration_hierarchy.GetConcreteTypeForLevel(1);
                             level_two_type = event_migration_hierarchy.GetConcreteTypeForLevel(2);
                         };

        It should_get_the_correct_type_for_level_one = () => level_one_type.ShouldEqual(typeof(SimpleEvent));
        It should_get_the_correct_type_for_level_two = () => level_two_type.ShouldEqual(typeof(Fakes.v3.SimpleEvent));
    }
}