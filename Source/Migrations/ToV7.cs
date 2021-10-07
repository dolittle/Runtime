// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Versioning;
using EventStore = Dolittle.Runtime.Events.Store.MongoDB.Migrations;
namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanMigrateDataStores" /> that can migrate from major version 6 to major version 7.
    /// /// </summary>
    public class ToV7 : ICanMigrateDataStores
    {
        readonly EventStore.ToV7.Migrator _eventStoreToV7;

        public ToV7(EventStore.ToV7.Migrator eventStoreToV7)
        {
            _eventStoreToV7 = eventStoreToV7;
        }

        /// <inheritdoc />
        public bool CanMigrateFor(Version from, Version to)
            => from.Major < 7 && to.Major == 7;

        /// <inheritdoc/>
        public EventStore.ICanMigrateAnEventStore EventStore => _eventStoreToV7;
    }
}
