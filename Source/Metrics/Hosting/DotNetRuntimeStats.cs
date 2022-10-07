// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Prometheus.DotNetRuntime;

namespace Dolittle.Runtime.Metrics.Hosting;

/// <summary>
/// Represents an <see cref="IHostedService"/> that owns the lifecycle of the dotnet runtime metrics collector <see cref="IDisposable"/>.
/// </summary>
public class DotNetRuntimeStats : IHostedService
{
    readonly CollectorRegistry _registry;
    IDisposable? _collector;

    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetRuntimeStats"/> class.
    /// </summary>
    /// <param name="registry">The <see cref="CollectorRegistry"/>.</param>
    public DotNetRuntimeStats(CollectorRegistry registry)
    {
        _registry = registry;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _collector = DotNetRuntimeStatsBuilder.Default().StartCollecting(_registry);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _collector?.Dispose();
        return Task.CompletedTask;
    }
}
