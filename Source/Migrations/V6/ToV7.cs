// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Migrations;
using Dolittle.Runtime.Events.Store.MongoDB.Migrations.V6.ToV7;
using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Migrations.V6
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanMigrateDataStores" /> that can migrate from major version 6 to major version 7.
    /// /// </summary>
    public class ToV7 : ICanMigrateDataStores
    {
        readonly Migrator _eventStoreToV7;

        public ToV7(Migrator eventStoreToV7)
        {
            _eventStoreToV7 = eventStoreToV7;

        }
        public bool CanMigrateFor(Version from, Version to)
            => from.Major == 6 && to.Major == 7;

        /// <inheritdoc/>
        public ICanMigrateAnEventStore EventStore => _eventStoreToV7;
    }
}
