// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Dolittle.Runtime.Domain.Tenancy;
using Integration.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Benchmarks;

/// <summary>
/// Represents a base for a <see cref="BenchmarkDotNet.Jobs.Job"/> that initializes a Runtime server for each benchmark.
/// </summary>
public abstract class JobBase
{
    RunningRuntime? _runtime;
    
    /// <summary>
    /// Ensures that a local MongoDB is reachable, and boots up a new Runtime Server host configured for tenants provided by the <see cref="GetTenantsToSetup"/>.
    /// Lastly calls the <see cref="Setup"/> method on implemented jobs with the <see cref="ServiceProvider"/> for the booted Runtime Server.
    /// </summary>
    [GlobalSetup]
    public void GlobalSetup()
    {
        _runtime = Runtime.CreateAndStart(NumTenants);
        Setup(_runtime.Host.Services);
    }

    /// <summary>
    /// Calls the <see cref="Cleanup"/> method on implemented jobs, stops the running Runtime Server host, and removes all created MongoDB databases.
    /// </summary>
    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Cleanup();
        Runtime.CleanAll(_runtime!).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets the <see cref="TenantId">tenants</see> configured for the running benchmark.
    /// </summary>
    protected IEnumerable<TenantId> ConfiguredTenants => _runtime!.ConfiguredTenants;

    /// <summary>
    /// Gets the number of tenants to setup.
    /// </summary>
    /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="TenantId"/> to configure.</returns>
    public virtual int NumTenants { get; set; } = 1;
    
    /// <summary>
    /// The method that sets up the environment for each benchmark to run.
    /// </summary>
    /// <param name="services">The services provided by the started Runtime Server host.</param>
    protected abstract void Setup(IServiceProvider services);

    /// <summary>
    /// The method that cleans up the environment after each benchmark run.
    /// </summary>
    protected abstract void Cleanup();
}
