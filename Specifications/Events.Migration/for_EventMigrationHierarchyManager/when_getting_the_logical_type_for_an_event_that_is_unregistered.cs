// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationHierarchyManager
{
    public class when_getting_the_logical_type_for_an_event_that_is_unregistered : given.an_event_migration_hierarchy_manager_with_two_logical_events
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => event_migration_hierarchy_manager.GetLogicalTypeFor(typeof(IEvent)));

        It should_throw_an_unregistered_event_exception = () => exception.ShouldBeOfExactType(typeof(UnregisteredEventException));
    }
}