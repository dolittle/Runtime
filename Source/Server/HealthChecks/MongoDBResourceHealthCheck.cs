// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dolittle.Runtime.Server.HealthChecks
{
    /// <summary>
    /// Represents an implementation of <see cref="ForAllTenantsHealthCheck"/> that checks that the MongoDB Read Model connection string is properly configured.
    /// </summary>
    public class MongoDBResourceHealthCheck : ForAllTenantsHealthCheck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDBResourceHealthCheck"/> class.
        /// </summary>
        /// <param name="forAllTenants">The performer to use to perform the health check for all tenants.</param>
        public MongoDBResourceHealthCheck(IPerformActionOnAllTenants forAllTenants)
            : base(forAllTenants)
        {
        }

        /// <inheritdoc />
        protected override Task<HealthCheckResult> CheckTenantHealthAsync(HealthCheckContext context, TenantId tenant, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Check if can get read-models for tenant? Is this needed for v6?
                // _getConnectionStringForTenant(tenant);
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Failed to get read model connection string. Maybe the resource is not configured correctly"));
            }
        }
    }
}