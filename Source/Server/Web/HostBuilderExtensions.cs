// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Dolittle.Runtime.Hosting;
using Dolittle.Runtime.Server.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Polly;

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
                        .AddCheck<MongoDBHealthCheck>("RuntimeMongoDBHealthCheck");
                    services.AddRouting();
                });
                webHost.Configure(app =>
                {
                    app.UseHealthChecks("/healthcheck",
                        new HealthCheckOptions
                        {
                            ResponseWriter = async (context, report) =>
                            {
                                var result = JsonConvert.SerializeObject(
                                    report,
                                    Formatting.Indented,
                                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new List<JsonConverter>{new StringEnumConverter()}});
                                context.Response.ContentType = MediaTypeNames.Application.Json;
                                await context.Response.WriteAsync(result);
                            }
                        });
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            }));
}
