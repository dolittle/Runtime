// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    services.AddHealthChecks();
                    services.AddRouting();
                });
                webHost.Configure(app =>
                {
                    app.UseHealthChecks("/healthz");
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            }));
}
