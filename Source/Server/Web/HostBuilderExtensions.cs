// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proto.Cluster;

namespace Dolittle.Runtime.Server.Web;

/// <summary>
/// Ex
/// </summary>
public static class HostBuilderExtensions
{
    public static IHostBuilder AddWebHost(this IHostBuilder builder)
        => builder
            .AddScopedHost(_ => _.ConfigureWebHost(webHost =>
            {
                webHost.UseKestrel();
                webHost.ConfigureServices(services =>
                {
                    services.AddKestrelConfiguration();
                    services.AddControllers();
                    services.AddHealthChecks()
                        .AddCheck<ClusterHealthCheck>("proto-cluster");
                    services.AddRouting();
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen(options =>
                    {
                        // TODO: Fix JSON serializer so that Web APIs don't need copies of types
                        options.SchemaGeneratorOptions.SchemaIdSelector = _ => _.FullName;
                    });
                });
                webHost.Configure(app =>
                {
                    app.UseHealthChecks("/healthz");

                    app.UseSwagger();
                    app.UseSwaggerUI();

                    app.UseRouting();
                    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
                });
            }));
}
