// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Versioning;

namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Defines a system that knows about data store migrations between different versions of the Runtime
    /// </summary>
    public interface IMigrations
    {
        /// <summary>
        /// Tries to get a migration that can convert data stores between the provided versions.
        /// </summary>
        /// <param name="from">The version to migrate from.</param>
        /// <param name="to">The version to migrate to.</param>
        /// <returns>A <see cref="Try{T}" /> of <see cref="ICanMigrateDataStores" /> with migrations if there exists one between specificed versions.</returns>
        /// <returns></returns>
        Try<ICanMigrateDataStores> GetFor(Version from, Version to);
    }
}
