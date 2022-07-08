// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Services.Configuration;
using Grpc.Health.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Services.HealthChecks;

/// <summary>
/// Represents an implementation of <see cref="IHealthCheck"/> that attempts to perform a <see cref="HealthCheckRequest"/> on a specific <see cref="EndpointVisibility"/>.
/// </summary>
[DisableAutoRegistration]
public class EndpointHealthCheck : IHealthCheck
{
    readonly EndpointVisibility _visibility;
    readonly Func<IOptions<EndpointsConfiguration>> _getConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointHealthCheck"/> class.
    /// </summary>
    /// <param name="visibility">The endpoint visibility to attempt to connect to.</param>
    /// <param name="getConfiguration">The factory to use to resolve the endpoint configuration.</param>
    public EndpointHealthCheck(EndpointVisibility visibility, Func<IOptions<EndpointsConfiguration>> getConfiguration)
    {
        _visibility = visibility;
        _getConfiguration = getConfiguration;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var configuration = _getConfiguration().Value.GetConfigurationFor(_visibility);
            var client = CreateClientFor(configuration);

            try
            {
                var request = new HealthCheckRequest();
                var response = await client.CheckAsync(request, cancellationToken: cancellationToken);

                return response.Status switch
                {
                    HealthCheckResponse.Types.ServingStatus.Serving => HealthCheckResult.Healthy("Endpoint responding to health checks"),
                    _ => HealthCheckResult.Unhealthy("Endpoint not responding to health checks"),
                };
            }
            catch (Exception exception)
            {
                return HealthCheckResult.Unhealthy($"Error occurred during health check request. {exception.Message}");
            }
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy("Could not get endpoint configuration. Maybe the endpoints is not configured correctly.");
        }
    }

    static Health.HealthClient CreateClientFor(EndpointConfiguration configuration)
        => new(GrpcChannel.ForAddress($"http://localhost:{configuration.Port}"));
}
