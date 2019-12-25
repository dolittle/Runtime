// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Migration.Specs.Fakes;
using Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationManager.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Migration.Specs.for_EventMigrationService.given
{
    public abstract class an_event_with_a_migrator : an_event_migrator_service_with_no_registered_migrators
    {
        Establish context = () => event_migrator_manager.RegisterMigrator(typeof(SimpleEventV1ToV2Migrator));
    }
}