// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Services.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Server.HealthChecks;

[Singleton]
public class GrpcHealthCheck : IHealthCheck
{
    readonly IServiceProvider _serviceProvider;
    readonly EndpointsConfiguration _endpoints;

    public GrpcHealthCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoints = _serviceProvider.GetRequiredService<IOptions<EndpointsConfiguration>>().Value;
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Failed to get endpoints configuration. {e.Message}");
        }
    }
    
}
