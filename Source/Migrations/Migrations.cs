// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Rudimentary;
using Microsoft.Extensions.DependencyInjection;
using Version = Dolittle.Runtime.Versioning.Version;

namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="IMigrations"/>.
    /// </summary>
    public class Migrations : IMigrations
    {
        readonly IEnumerable<ICanMigrateDataStores> _migrators;


        public Migrations(IServiceProvider container)
        {
            _migrators = container.GetServices<ICanMigrateDataStores>();
        }

        /// <inheritdoc/>
        public Try<ICanMigrateDataStores> GetFor(Version from, Version to)
        {
            var migrators = _migrators.Where(_ => _.CanMigrateFor(from, to));
            switch (migrators.Count())
            {
                case 0:
                    return new NoMigratorDefinedBetweenVersions(from, to);
                case 1:
                    return Try<ICanMigrateDataStores>.Succeeded(migrators.First());
                default:
                    return new MultipleMigratorsDefinedBetweenVersions(from, to);
            }
        }
    }
}
