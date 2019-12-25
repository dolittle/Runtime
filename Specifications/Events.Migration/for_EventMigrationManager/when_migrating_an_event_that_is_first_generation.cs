// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Events;
using Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationManager.given;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationService
{
    class when_migrating_an_event_that_is_first_generation : an_event_migrator_service_with_no_registered_migrators
    {
        static IEvent result;

        Because of = () => result = event_migrator_manager.Migrate(source_event);

        It should_return_the_same_instance_of_the_event = () => result.ShouldBeTheSameAs(source_event);
    }
}