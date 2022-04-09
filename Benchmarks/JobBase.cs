// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using BenchmarkDotNet.Attributes;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Metrics.Hosting;
using Dolittle.Runtime.Server.Web;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Benchmarks;

/// <summary>
/// Represents a base for a <see cref="BenchmarkDotNet.Jobs.Job"/> that initializes a Runtime server for each benchmark.
/// </summary>
public abstract class JobBase
{
    static readonly MicroserviceId _microserviceId = "1567f58f-c58e-4752-bf3d-b8a89deb78d0";
    static readonly Environment _environment = "Benchmarking";

    static readonly Command<BsonDocument> _pingCommand = new BsonDocument("ping", 1);

    IMongoClient _mongoClient;
    List<string> _databases;
    IHost _runtimeHost;
    
    /// <summary>
    /// Ensures that a local MongoDB is reachable, and boots up a new Runtime Server host configured for tenants provided by the <see cref="GetTenantsToSetup"/>.
    /// Lastly calls the <see cref="Setup"/> method on implemented jobs with the <see cref="ServiceProvider"/> for the booted Runtime Server.
    /// </summary>
    [GlobalSetup]
    public void GlobalSetup()
    {
        _mongoClient = new MongoClient("mongodb://localhost:27017");
        ThrowIfCannotConnectToMongoDB();
        
        _databases = new List<string>();
        var configuration = new Dictionary<string, string>();
        CreateRuntimeConfiguration(configuration);

        _runtimeHost = Host.CreateDefaultBuilder()
            .UseDolittleServices()
            .ConfigureHostConfiguration(_ => _
                .AddInMemoryCollection(configuration))
            .ConfigureServices(_ => _
                .AddLogging(_ => _
                    .ClearProviders()))
            .AddActorSystem()
            // .AddMetrics()
            .AddGrpcHost(EndpointVisibility.Private)
            .AddGrpcHost(EndpointVisibility.Public)
            .AddGrpcHost(EndpointVisibility.Management)
            // .AddMetricsHost()
            // .AddWebHost()
            .Build();
        
        _runtimeHost.StartAsync().GetAwaiter().GetResult();
        Setup(_runtimeHost.Services);
    }

    /// <summary>
    /// Calls the <see cref="Cleanup"/> method on implemented jobs, stops the running Runtime Server host, and removes all created MongoDB databases.
    /// </summary>
    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Cleanup();
        _runtimeHost.StopAsync().GetAwaiter().GetResult();
        DropAllDatabases();
    }
    
    /// <summary>
    /// Gets the <see cref="TenantId">tenants</see> configured for the running benchmark.
    /// </summary>
    protected IEnumerable<TenantId> ConfiguredTenants { get; private set; }

    /// <summary>
    /// Method to override to define the <see cref="TenantId">tenants</see> to run configure for the running benchmark.
    /// </summary>
    /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="TenantId"/> to configure.</returns>
    protected virtual IEnumerable<TenantId> GetTenantsToSetup()
        => new[] {new TenantId(Guid.NewGuid())};

    /// <summary>
    /// Creates a new <see cref="Dolittle.Runtime.Execution.ExecutionContext"/> for the specified tenant to be used in the benchmark.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/> to create an execution context for.</param>
    /// <returns>The newly created <see cref="Dolittle.Runtime.Execution.ExecutionContext"/>.</returns>
    protected static ExecutionContext CreateExecutionContextFor(TenantId tenant)
        => new(_microserviceId, tenant, Version.NotSet, _environment, CorrelationId.New(), Claims.Empty, CultureInfo.InvariantCulture);

    /// <summary>
    /// The method that sets up the environment for each benchmark to run.
    /// </summary>
    /// <param name="services">The services provided by the started Runtime Server host.</param>
    protected abstract void Setup(IServiceProvider services);

    /// <summary>
    /// The method that cleans up the environment after each benchmark run.
    /// </summary>
    protected abstract void Cleanup();

    void CreateRuntimeConfiguration(Dictionary<string, string> configuration)
    {
        configuration["dolittle:runtime:eventstore:backwardscompatibility:version"] = "V7";
        configuration["dolittle:runtime:platform:microserviceID"] = _microserviceId.ToString();
        configuration["dolittle:runtime:platform:environment"] = _environment.ToString();
        
        ConfiguredTenants = GetTenantsToSetup();
        foreach (var tenant in ConfiguredTenants)
        {
            var eventStoreName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:eventStore:servers:0"] = "localhost";
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:eventStore:database"] = eventStoreName;
            _databases.Add(eventStoreName);
            
            var projectionsName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:projections:servers:0"] = "localhost";
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:projections:database"] = projectionsName;
            _databases.Add(projectionsName);
            
            var embeddingsName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:embeddings:servers:0"] = "localhost";
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:embeddings:database"] = embeddingsName;
            _databases.Add(embeddingsName);
            
            var readModelsName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:readModels:host"] = "mongodb://localhost:27017";
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:readModels:database"] = readModelsName;
            _databases.Add(readModelsName);
        }
    }

    void ThrowIfCannotConnectToMongoDB()
    {
        try
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
            _mongoClient.GetDatabase("admin").RunCommand(_pingCommand, cancellationToken: cts.Token);
        }
        catch (Exception)
        {
            throw new MongoException("Could not connect to a local MongoDB, please make sure you have one running at localhost:27017");
        }
    }

    void DropAllDatabases()
    {
        foreach (var database in _databases)
        {
            _mongoClient.DropDatabase(database);
        }
    }
}
