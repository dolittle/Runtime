// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Migration.Specs.Fakes;
using Dolittle.Types;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationManager.given
{
    public abstract class an_event_migrator_service_with_no_registered_migrators
    {
        protected static SimpleEvent source_event;
        protected static Guid event_source_id;
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;
        protected static EventMigratorManager event_migrator_manager;

        Establish context = () =>
                                {
                                    event_source_id = Guid.NewGuid();
                                    source_event = new SimpleEvent();
                                    type_finder = new Mock<ITypeFinder>();
                                    container = new Mock<IContainer>();
                                    container.Setup(c => c.Get(Moq.It.IsAny<Type>())).Returns(
                                        (Type t) => Activator.CreateInstance(t));

                                    event_migrator_manager = new EventMigratorManager(type_finder.Object, container.Object);
                                };
    }
}
