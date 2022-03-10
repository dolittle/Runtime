// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Resources.MongoDB;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using EventStoreConnection = Dolittle.Runtime.Events.Store.MongoDB.IDatabaseConnection;
using ProjectionsConnection = Dolittle.Runtime.Projections.Store.MongoDB.IDatabaseConnection;
using EmbeddingsConnection = Dolittle.Runtime.Embeddings.Store.MongoDB.IDatabaseConnection;

namespace Dolittle.Runtime.Server.HealthChecks;

[Singleton]
public class MongoDBHealthCheck : IHealthCheck
{
    readonly IPerformActionsForAllTenants _forAllTenants;
    readonly ITenants _tenants;
    readonly Func<TenantId, EventStoreConnection> _eventStoreConnectionForTenant;
    readonly Func<TenantId, ProjectionsConnection> _projectionsConnectionForTenant;
    readonly Func<TenantId, EmbeddingsConnection> _embeddingsConnectionForTenant;
    readonly Func<TenantId, IKnowTheConnectionString> _readModelsConnectionStringForTenant;

    public MongoDBHealthCheck(
        IPerformActionsForAllTenants forAllTenants,
        ITenants tenants,
        Func<TenantId, EventStoreConnection> eventStoreConnectionForTenant,
        Func<TenantId, ProjectionsConnection> projectionsConnectionForTenant,
        Func<TenantId, EmbeddingsConnection> embeddingsConnectionForTenant,
        Func<TenantId, IKnowTheConnectionString> readModelsConnectionStringForTenant)
    {
        _forAllTenants = forAllTenants;
        _tenants = tenants;
        _eventStoreConnectionForTenant = eventStoreConnectionForTenant;
        _projectionsConnectionForTenant = projectionsConnectionForTenant;
        _embeddingsConnectionForTenant = embeddingsConnectionForTenant;
        _readModelsConnectionStringForTenant = readModelsConnectionStringForTenant;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var errors = new ConcurrentDictionary<TenantId, List<string>>();
        await _forAllTenants.PerformAsync((tenantId, _) => CheckConnectionForAll(errors, tenantId, cancellationToken)).ConfigureAwait(false);

        if (errors.IsEmpty)
        {
            return HealthCheckResult.Healthy("The Runtime is ready and set up correctly");
        }
        return HealthCheckResult.Unhealthy(string.Join('\n', errors.Select(tenantAndErrors =>
        {
            var (tenant, errors) = tenantAndErrors;
            return $"\n{string.Join('\n', errors.Select(error => $"For tenant {tenant} {error}"))}";
        })), data: new ReadOnlyDictionary<string, object>(errors.ToDictionary(_ => _.Key.Value.ToString(), _ => (object)_.Value)));
    }

    static void AddError(ConcurrentDictionary<TenantId, List<string>> errors, TenantId tenantId, string error)
        => errors.AddOrUpdate(
            tenantId,
            new List<string>
            {
                error
            },
            (_, list) => list.Append(error).ToList());

    Task CheckConnectionForAll(
        ConcurrentDictionary<TenantId, List<string>> errors,
        TenantId tenantId,
        CancellationToken cancellationToken)
        => Task.WhenAll(
            CheckConnectionFor(
                errors,
                "event store",
                tenantId,
                _eventStoreConnectionForTenant,
                connection => connection.Database,
                cancellationToken),
            CheckConnectionFor(
                errors,
                "projections store",
                tenantId,
                _projectionsConnectionForTenant,
                connection => connection.Database,
                cancellationToken),
            CheckConnectionFor(
                errors,
                "embeddings store",
                tenantId,
                _embeddingsConnectionForTenant,
                connection => connection.Database,
                cancellationToken),
            CheckReadModelsConnection(errors, tenantId, cancellationToken)
        );

    static async Task CheckConnectionFor<TConnection>(
        ConcurrentDictionary<TenantId, List<string>> errors,
        string storeName,
        TenantId tenantId,
        Func<TenantId, TConnection> getConnection,
        Func<TConnection,IMongoDatabase> getDatabase,
        CancellationToken cancellationToken)
    {
        try
        {
            var connection = getConnection(tenantId);
            var database = getDatabase(connection);
            try
            {
                await PingDatabase(database, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                AddError(errors, tenantId, $"failed to establish connection to {storeName} to database {database.DatabaseNamespace} on address {database.Client.Settings.Server}");
            }
        }
        catch (Exception)
        {
            AddError(errors, tenantId, $"failed to get {storeName} connection configuration. Maybe {storeName} resource is not configured for tenant or the configuration is wrongly formatted");
        }
    }
    async Task CheckReadModelsConnection(
        ConcurrentDictionary<TenantId, List<string>> errors,
        TenantId tenantId,
        CancellationToken cancellationToken)
    {
        try
        {
            var connectionString = _readModelsConnectionStringForTenant(tenantId).ConnectionString;
            try
            {
                var clientSettings = MongoClientSettings.FromUrl(connectionString);
                var client = new MongoClient(clientSettings.Freeze());
                var database = client.GetDatabase(connectionString.DatabaseName);
                await PingDatabase(database, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                AddError(errors, tenantId, $"failed to establish connection to read models with connection string {connectionString}");
            }
        }
        catch (Exception)
        {
            AddError(errors, tenantId, "failed to get read models connection configuration. Maybe read models resource is not configured for tenant or the configuration is wrongly formatted");
        }
    }

    static async Task PingDatabase(IMongoDatabase database, CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(2));
        await database.RunCommandAsync((Command<BsonDocument>) "{ping:1}", cancellationToken: cts.Token).ConfigureAwait(false);
    }
}
