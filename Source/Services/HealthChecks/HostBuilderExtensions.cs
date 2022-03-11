// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Services.Configuration;
using Grpc.Health.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Services.HealthChecks;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/> related to gRPC endpoint health checks.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Configures a <see cref="HealthCheckRegistration"/> that performs a gRPC <see cref="HealthCheckRequest"/> on the specified <see cref="EndpointVisibility"/>.
    /// </summary>
    /// <param name="builder">The host builder to configure.</param>
    /// <param name="visibility">The endpoint visibility to add a health check for.</param>
    /// <returns>The builder for continuation.</returns>
    public static IHostBuilder AddGrpcEndpointHealthCheck(this IHostBuilder builder, EndpointVisibility visibility)
        => builder.ConfigureServices(services =>
            services.AddTransient<IConfigureOptions<HealthCheckServiceOptions>>(_ => new HealthCheckConfiguration(
                visibility,
                _.GetRequiredService<Func<IOptions<EndpointsConfiguration>>>(),
                _.GetRequiredService<ILogger<HealthCheckConfiguration>>())));
}
