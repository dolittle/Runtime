// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Security;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Environment = Dolittle.Runtime.Execution.Environment;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Versioning.Version;

namespace Dolittle.Runtime.Server.HealthChecks
{
    /// <summary>
    /// Represents an abstract implementation of <see cref="IHealthCheck"/> that performs health checks for all tenants and aggregates the results.
    /// </summary>
    public abstract class ForAllTenantsHealthCheck : IHealthCheck
    {
        readonly FactoryFor<TenantsConfiguration> _getTenants;
        readonly IExecutionContextManager _executionContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForAllTenantsHealthCheck"/> class.
        /// </summary>
        /// <param name="forAllTenants">The performer to use to perform the health check for each tenant.</param>
        protected ForAllTenantsHealthCheck(FactoryFor<TenantsConfiguration> getTenants, IExecutionContextManager executionContextManager)
        {
            _getTenants = getTenants;
            _executionContextManager = executionContextManager;
        }

        /// <inheritdoc />
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (!TryGetTenants(out var tenants, out var failedHealthCheckResult))
            {
                return failedHealthCheckResult;
            }
            var results = await PerformChecksForAllTenants(tenants, context, cancellationToken).ConfigureAwait(false);
            var data = new Dictionary<string, object>();

            var aggregateStatus = HealthStatus.Healthy;
            var nonHealthyTenants = new List<string>();
            foreach (var (tenant, result) in results)
            {
                aggregateStatus = ReduceTenantStatus(aggregateStatus, result.Status);
                data[tenant.Value.ToString()] = result.Description;

                if (result.Status != HealthStatus.Healthy)
                {
                    nonHealthyTenants.Add(tenant.Value.ToString());
                }
            }

            var description = aggregateStatus == HealthStatus.Healthy
                ? null
                : $"Health checks failed for tenants {string.Join(", ", nonHealthyTenants)}";

            return new HealthCheckResult(aggregateStatus, description, data: data);
        }

        /// <summary>
        /// Runs the health check for a specific tenant, returning the status of the component being checked.
        /// </summary>
        /// <param name="context">A context object associated with the current execution.</param>
        /// <param name="tenant">The tenant to check the health for.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check.</param>
        /// <returns>A <see cref="Task{TResult}"/> that completes when the health check has finished, yielding the status of the component being checked.</returns>
        protected abstract Task<HealthCheckResult> CheckTenantHealthAsync(HealthCheckContext context, TenantId tenant, CancellationToken cancellationToken);

        async Task<IDictionary<TenantId, HealthCheckResult>> PerformChecksForAllTenants(TenantId[] tenants, HealthCheckContext context, CancellationToken cancellationToken)
        {
            var resultsPerTenant = new Dictionary<TenantId, HealthCheckResult>();

            var baseExecutionContext = _executionContextManager.CurrentFor(new ExecutionContext(
                Microservice.NotSet,
                TenantId.System,
                Version.NotSet,
                Environment.Undetermined,
                CorrelationId.System,
                Claims.Empty,
                CultureInfo.InvariantCulture));
            try
            {
                foreach (var tenant in tenants)
                {
                    _executionContextManager.CurrentFor(tenant);
                    resultsPerTenant.Add(tenant, await CheckTenantHealthAsync(context, tenant, cancellationToken).ConfigureAwait(false));
                }
            }
            finally
            {
                _executionContextManager.CurrentFor(baseExecutionContext);
            }
            return resultsPerTenant;
        }

        bool TryGetTenants(out TenantId[] tenants, out HealthCheckResult failedHealthCheckResult)
        {
            tenants = Array.Empty<TenantId>();
            failedHealthCheckResult = default;
            try
            {
                tenants = _getTenants().Keys.Select(_ => new TenantId(_)).ToArray();
                return true;
            }
            catch (Exception)
            {
                failedHealthCheckResult = HealthCheckResult.Unhealthy("Failed to get tenants configuration. Maybe tenants.json is formatted wrongly");
                return false;
            }
        }

        static HealthStatus ReduceTenantStatus(HealthStatus aggregateStatus, HealthStatus tenantStatus)
            => tenantStatus switch
            {
                HealthStatus.Unhealthy => HealthStatus.Unhealthy,
                HealthStatus.Degraded when aggregateStatus != HealthStatus.Unhealthy => HealthStatus.Degraded,
                _ => aggregateStatus,
            };
    }
}

