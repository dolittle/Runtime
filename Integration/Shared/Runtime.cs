// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors.Hosting;
using Dolittle.Runtime.Bootstrap.Hosting;
using Dolittle.Runtime.DependencyInversion.Building;
using Dolittle.Runtime.Diagnostics.OpenTelemetry;
using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Metrics.Hosting;
using Dolittle.Runtime.Server.Web;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.Configuration;
using Dolittle.Runtime.Services.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Environment = Dolittle.Runtime.Domain.Platform.Environment;
using Version = Dolittle.Runtime.Domain.Platform.Version;

namespace Integration.Shared;

/// <summary>
/// Represents a base for a <see cref="BenchmarkDotNet.Jobs.Job"/> that initializes a Runtime server for each benchmark.
/// </summary>
public static class Runtime
{
    static readonly MicroserviceId _microserviceId = "1567f58f-c58e-4752-bf3d-b8a89deb78d0";
    static readonly Environment _environment = "Benchmarking";

    static readonly Command<BsonDocument> _pingCommand = new BsonDocument("ping", 1);

    public static RunningRuntime CreateAndStart(int numberOfTenants)
    {
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        ThrowIfCannotConnectToMongoDB(mongoClient);

        var cfg = new ConfigurationManager()
            .AddEnvironmentVariables()
            .Build();

        var configuration = new Dictionary<string, string?>();
        var (databases, tenants) = CreateRuntimeConfiguration(configuration, numberOfTenants);

        var runtimeHost = Host.CreateDefaultBuilder()
            .UseDolittleServices()
            .ConfigureOpenTelemetry(cfg)
            .ConfigureHostConfiguration(builder =>
            {
                builder.Sources.Clear();
                builder.AddInMemoryCollection(configuration);
            })
            .ConfigureAppConfiguration(builder =>
            {
                builder.Sources.Clear();
                builder.AddInMemoryCollection(configuration);
            })
            .ConfigureServices(coll =>
            {
                coll.AddLogging(builder => builder.ClearProviders());
                coll.AddOptions<EndpointsConfiguration>().Configure(builder =>
                {
                    builder.Management = new EndpointConfiguration { Port = 0 };
                    // builder.Private = new EndpointConfiguration { Port = 0 };
                    builder.Public = new EndpointConfiguration { Port = 0 };
                });
            })
            .AddActorSystem()
            .AddMetrics()
            .AddGrpcHost(EndpointVisibility.Private)
            .AddGrpcHost(EndpointVisibility.Public)
            .AddGrpcHost(EndpointVisibility.Management)
            .AddMetricsHost()
            .AddWebHost()
            .Build();

        runtimeHost.PerformBootstrap().GetAwaiter().GetResult();
        runtimeHost.Start();


        return new RunningRuntime(runtimeHost, tenants, mongoClient, databases);
    }

    public static Task CleanAll(RunningRuntime runtime)
        => Task.WhenAll(runtime.Host.StopAsync(), DropAllDatabases(runtime));

    public static Task DropAllDatabases(RunningRuntime runtime)
        => Task.WhenAll(runtime.Databases.Select(database => runtime.MongoClient.DropDatabaseAsync(database, CancellationToken.None)));


    /// <summary>
    /// Creates a new <see cref="Dolittle.Runtime.Execution.ExecutionContext"/> for the specified tenant to be used in the benchmark.
    /// </summary>
    /// <param name="tenant">The <see cref="TenantId"/> to create an execution context for.</param>
    /// <returns>The newly created <see cref="Dolittle.Runtime.Execution.ExecutionContext"/>.</returns>
    public static Dolittle.Runtime.Execution.ExecutionContext CreateExecutionContextFor(TenantId tenant)
        => new(_microserviceId, tenant, Version.NotSet, _environment, Guid.NewGuid(), null, Claims.Empty, CultureInfo.InvariantCulture);

    static (IEnumerable<string> databases, IEnumerable<TenantId> tenants) CreateRuntimeConfiguration(Dictionary<string, string?> configuration,
        int numberOfTenants)
    {
        configuration["dolittle:runtime:platform:microserviceID"] = _microserviceId.ToString();
        configuration["dolittle:runtime:platform:environment"] = _environment.ToString();

        var tenants = Enumerable.Range(0, numberOfTenants).Select(_ => new TenantId(Guid.NewGuid())).ToArray();
        var databases = new List<string>();
        foreach (var tenant in tenants)
        {
            var eventStoreName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:eventStore:servers:0"] = "localhost";
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:eventStore:database"] = eventStoreName;
            databases.Add(eventStoreName);

            var readModelsName = Guid.NewGuid().ToString();
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:readModels:host"] = "mongodb://localhost:27017";
            configuration[$"dolittle:runtime:tenants:{tenant}:resources:readModels:database"] = readModelsName;
            databases.Add(readModelsName);
        }

        return (databases, tenants);
    }

    static void ThrowIfCannotConnectToMongoDB(IMongoClient mongoClient)
    {
        try
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
            mongoClient.GetDatabase("admin").RunCommand(_pingCommand, cancellationToken: cts.Token);
        }
        catch (Exception)
        {
            throw new MongoException("Could not connect to a local MongoDB, please make sure you have one running at localhost:27017");
        }
    }
}
