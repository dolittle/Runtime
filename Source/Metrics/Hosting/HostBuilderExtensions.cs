// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Hosting;
using Dolittle.Runtime.Metrics.DependencyInversion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Prometheus.DotNetRuntime;

namespace Dolittle.Runtime.Metrics.Hosting;

/// <summary>
/// Ex
/// </summary>
public static class HostBuilderExtensions
{
    public static IHostBuilder AddMetrics(this IHostBuilder builder)
        => builder.ConfigureServices(services =>
        {
            var registry = Prometheus.Metrics.NewCustomRegistry();
            DotNetStats.Register(registry);
            var collector = DotNetRuntimeStatsBuilder.Default().StartCollecting(registry);

            var factory = Prometheus.Metrics.WithCustomRegistry(registry);
            services.AddSingleton<IMetricFactory>(factory);
            
            services.AddSingleton(provider =>
            {
                foreach (var collector in provider.GetRequiredService<IEnumerable<MetricCollector>>())
                {
                    provider.GetRequiredService(collector.CollectorType);
                }

                return registry;
            });
        });
    
    public static IHostBuilder AddMetricsHost(this IHostBuilder builder)
        => builder
            .AddMetrics()
            .AddScopedHost(_ => _.ConfigureWebHost(metricsHost =>
            {
                metricsHost.UseKestrel();

                metricsHost.ConfigureServices(services =>
                {
                    services.AddKestrelConfiguration();
                    services.AddRouting();
                });
                metricsHost.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/", () => "Hello from metrics - yes this is dog!");
                        endpoints.MapMetrics(registry: app.ApplicationServices.GetRequiredService<CollectorRegistry>());
                    });
                });
            }));
}
