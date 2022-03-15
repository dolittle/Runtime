// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.MongoDB;
using Dolittle.Runtime.Tenancy;
using MongoDB.Driver;

namespace Dolittle.Runtime.Server.HealthChecks
{
    /// <summary>
    /// Represents an implementation of <see cref="MongoDatabaseHealthCheck"/> that checks the Projection Store database.
    /// </summary>
    public class ProjectionStoreHealthCheck : MongoDatabaseHealthCheck
    {
        readonly FactoryFor<DatabaseConnection> _getConnectionForTenant;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionStoreHealthCheck"/> class.
        /// </summary>
        /// <param name="forAllTenants">The performer to use to perform the health check for all tenants.</param>
        /// <param name="getConnectionForTenant">The factory to use to get the Event Store connection for a tenant.</param>
        public ProjectionStoreHealthCheck(IPerformActionOnAllTenants forAllTenants, FactoryFor<DatabaseConnection> getConnectionForTenant)
            : base(forAllTenants)
        {
            _getConnectionForTenant = getConnectionForTenant;
        }

        /// <inheritdoc />
        protected override IMongoDatabase GetDatabaseFor(TenantId tenant)
            => _getConnectionForTenant().Database;
    }
}