// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.Serialization.Json;
namespace Dolittle.Runtime.Migrations
{
    /// <summary>
    /// Represents an implementation of <see cref="IMigrationPerformers"/>.
    /// </summary>
    public class MigrationPerformers : IMigrationPerformers
    {
        readonly ISerializer _serializer;
        public MigrationPerformers(ISerializer serializer)
        {
            _serializer = serializer;

        }
        /// <inheritdoc />
        public IPerformMigrations ConfiguredFor(ResourceConfigurationsByTenant resourceConfiguration)
            => new MigrationPerformer(new ResourceConfigurationsByTenantProvider(resourceConfiguration, _serializer), resourceConfiguration);
    }
}