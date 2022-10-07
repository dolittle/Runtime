// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Prometheus.DotNetRuntime;

namespace Dolittle.Runtime.Metrics.Hosting;

public class DotNetRuntimeStats : IHostedService
{
    readonly CollectorRegistry _registry;
    IDisposable _collector;

    public DotNetRuntimeStats(CollectorRegistry registry)
    {
        _registry = registry;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => _collector = DotNetRuntimeStatsBuilder.Default().StartCollecting(_registry);
    public Task StopAsync(CancellationToken cancellationToken) => _collector.Dispose();
}
