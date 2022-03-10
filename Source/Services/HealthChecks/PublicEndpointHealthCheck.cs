// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Services.Configuration;
using Grpc.Health.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Services.HealthChecks;

[Singleton]
public class PublicEndpointHealthCheck : IHealthCheck
{
    readonly IServiceProvider _serviceProvider;
    readonly EndpointsConfiguration _endpoints;

    public PublicEndpointHealthCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var publicEndpoint = _serviceProvider.GetRequiredService<IOptions<EndpointsConfiguration>>().Value.Public;

            var channel = GrpcChannel.ForAddress($"http://localhost:{publicEndpoint.Port}");
            var client = new Health.HealthClient(channel);
            var response = await client.CheckAsync(new HealthCheckRequest());
            return response.Status == HealthCheckResponse.Types.ServingStatus.Serving
                ? HealthCheckResult.Healthy($"Public Grpc endpoint on port {publicEndpoint.Port} is up and running")
                : HealthCheckResult.Unhealthy($"Public Grpc endpoint on port {publicEndpoint.Port} is not serving");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Failed to get public endpoints configuration. {e.Message}");
        }
    }
}

public interface IPerformGrpcHealthCheck
{
    public Task<HealthCheckResult> CheckHealthFor(string visibility, Func<IServiceProvider, int> getPortFromConfiguration);
}

public class PerformGrpcHealthCheck : IPerformGrpcHealthCheck
{
    readonly IServiceProvider _serviceProvider;

    public PerformGrpcHealthCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthFor(string visibility, Func<IServiceProvider, int> getPortFromConfiguration)
    {
        try
        {
            var publicEndpoint = getPortFromConfiguration(_serviceProvider);

            var channel = GrpcChannel.ForAddress($"http://localhost:{publicEndpoint}");
            var client = new Health.HealthClient(channel);
            var response = await client.CheckAsync(new HealthCheckRequest());
            return response.Status == HealthCheckResponse.Types.ServingStatus.Serving
                ? HealthCheckResult.Healthy($"{visibility} Grpc endpoint on port {publicEndpoint} is up and running")
                : HealthCheckResult.Unhealthy($"{visibility} Grpc endpoint on port {publicEndpoint} is not serving");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy($"Failed to get {visibility} endpoints configuration. {e.Message}");
        }
    }
}
