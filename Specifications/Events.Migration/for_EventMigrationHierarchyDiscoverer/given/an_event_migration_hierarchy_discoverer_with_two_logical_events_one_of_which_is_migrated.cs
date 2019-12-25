// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Events;
using Dolittle.Runtime.Events.Migration.Specs.Fakes;
using Dolittle.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationLevelDiscoverer.given
{
    public class an_event_migration_hierarchy_discoverer_with_two_logical_events_one_of_which_is_migrated
    {
        protected static EventMigrationHierarchyDiscoverer event_migration_level_discoverer;
        protected static Mock<ITypeFinder> type_finder;
        protected static Type[] event_types;

        Establish context = () =>
                                {
                                    event_types = new[]
                                                      {
                                                         typeof(AnotherSimpleEvent),
                                                         typeof(SimpleEvent),
                                                         typeof(Fakes.v2.SimpleEvent),
                                                         typeof(Fakes.v3.SimpleEvent)
                                                      };

                                    type_finder = new Mock<ITypeFinder>();
                                    type_finder.Setup(d => d.FindMultiple<IEvent>()).Returns(event_types);
                                    event_migration_level_discoverer = new EventMigrationHierarchyDiscoverer(type_finder.Object);
                                };
    }
}