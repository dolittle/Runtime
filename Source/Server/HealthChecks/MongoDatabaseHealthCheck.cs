// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Dolittle.Runtime.Server.HealthChecks;

/// <summary>
/// Represents an abstract implementation of <see cref="ForAllTenantsHealthCheck"/> that attempts to ping a MongoDB database.
/// </summary>
public abstract class MongoDatabaseHealthCheck : ForAllTenantsHealthCheck
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDatabaseHealthCheck"/> class.
    /// </summary>
    /// <param name="forAllTenants">The performer to use to perform the health check for each tenant.</param>
    protected MongoDatabaseHealthCheck(IPerformActionsForAllTenants forAllTenants)
        : base(forAllTenants)
    {
    }

    /// <inheritdoc />
    protected override async Task<HealthCheckResult> CheckTenantHealthAsync(HealthCheckContext context, TenantId tenant, CancellationToken cancellationToken)
    {
        try
        {
            var database = GetDatabaseFor(tenant);
            try
            {
                await PingDatabase(database, cancellationToken).ConfigureAwait(false);
                return HealthCheckResult.Healthy();
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy($"Failed to connect to database {database.DatabaseNamespace} on address {database.Client.Settings.Server}");
            }
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy($"Failed to get database. Maybe the resource is not configured correctly.");
        }
    }

    /// <summary>
    /// Gets the <see cref="IMongoDatabase"/> to try to connect to for the specified <see cref="TenantId"/>.
    /// </summary>
    /// <param name="tenant">The tenant to get the database for.</param>
    /// <returns>The configured MongoDB database for the specified tenant.</returns>
    protected abstract IMongoDatabase GetDatabaseFor(TenantId tenant);
    
    static async Task PingDatabase(IMongoDatabase database, CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(2));
        await database.RunCommandAsync((Command<BsonDocument>) "{ping: 1}", cancellationToken: cts.Token).ConfigureAwait(false);
    }
}
