// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.ResourceTypes.Configuration;
namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Defines a system that knows about <see cref="IPerformMigrations"/>.
    /// </summary>
    public interface IMigrationPerformers
    {
        /// <summary>
        /// Creates a new <see cref="IPerformMigrations"/> that is configured for the given <see cref="ResourceConfigurationsByTenant"/>.
        /// </summary>
        /// <param name="resourceConfiguration">The <see cref="ResourceConfigurationsByTenant"/>.</param>
        /// <returns>The configured <see cref="IPerformMigrations"/> instance.</returns>
        IPerformMigrations ConfiguredFor(ResourceConfigurationsByTenant resourceConfiguration);
    }
}