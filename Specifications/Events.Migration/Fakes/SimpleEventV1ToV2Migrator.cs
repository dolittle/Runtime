// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Migration.Specs.Fakes
{
    public class SimpleEventV1ToV2Migrator : IEventMigrator<SimpleEvent, v2.SimpleEvent>
    {
        public v2.SimpleEvent Migrate(SimpleEvent source)
        {
            var simpleEvent2 = new v2.SimpleEvent();
            return simpleEvent2;
        }
    }
}