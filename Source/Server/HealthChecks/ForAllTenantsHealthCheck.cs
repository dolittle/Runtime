// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Dolittle.Runtime.Server.HealthChecks;

/// <summary>
/// Represents an abstract implementation of <see cref="IHealthCheck"/> that performs health checks for all tenants and aggregates the results.
/// </summary>
public abstract class ForAllTenantsHealthCheck : IHealthCheck
{
    readonly IPerformActionsForAllTenants _forAllTenants;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForAllTenantsHealthCheck"/> class.
    /// </summary>
    /// <param name="forAllTenants">The performer to use to perform the health check for each tenant.</param>
    protected ForAllTenantsHealthCheck(IPerformActionsForAllTenants forAllTenants)
    {
        _forAllTenants = forAllTenants;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var results = await PerformChecksForAllTenants(context, cancellationToken).ConfigureAwait(false);
        var data = new Dictionary<string, object>();

        var aggregateStatus = HealthStatus.Healthy;
        var nonHealthyTenants = new List<TenantId>();
        foreach (var (tenant, result) in results)
        {
            aggregateStatus = ReduceTenantStatus(aggregateStatus, result.Status);
            data[tenant.ToString()] = result.Description;

            if (result.Status != HealthStatus.Healthy)
            {
                nonHealthyTenants.Add(tenant);
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

    async Task<IDictionary<TenantId, HealthCheckResult>> PerformChecksForAllTenants(HealthCheckContext context, CancellationToken cancellationToken)
    {
        var resultsPerTenant = new Dictionary<TenantId, HealthCheckResult>();
        await _forAllTenants.PerformAsync(async (tenant, _) =>
                resultsPerTenant.Add(tenant, await CheckTenantHealthAsync(context, tenant, cancellationToken).ConfigureAwait(false)))
            .ConfigureAwait(false);
        return resultsPerTenant;
    }

    static HealthStatus ReduceTenantStatus(HealthStatus aggregateStatus, HealthStatus tenantStatus)
        => tenantStatus switch
        {
            HealthStatus.Unhealthy => HealthStatus.Unhealthy,
            HealthStatus.Degraded when aggregateStatus != HealthStatus.Unhealthy => HealthStatus.Degraded,
            _ => aggregateStatus,
        };
}
