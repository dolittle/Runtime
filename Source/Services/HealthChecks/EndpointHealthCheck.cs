// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Services.HealthChecks
{
    /// <summary>
    /// Represents an implementation of <see cref="IHealthCheck"/> that attempts to perform a <see cref="HealthCheckRequest"/> on a specific <see cref="EndpointVisibility"/>.
    /// </summary>
    public class EndpointHealthCheck : IHealthCheck
    {
        readonly EndpointConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointHealthCheck"/> class.
        /// </summary>
        /// <param name="configuration">The endpoint configuration.</param>
        public EndpointHealthCheck(EndpointConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = CreateClient();
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

        Health.HealthClient CreateClient()
        {
            var keepAliveTime = new ChannelOption("grpc.keepalive_time", 1000);
            var keepAliveTimeout = new ChannelOption("grpc.keepalive_timeout_ms", 500);
            var keepAliveWithoutCalls = new ChannelOption("grpc.keepalive_permit_without_calls", 1);
            var channel = new Channel(
                "localhost",
                _configuration.Port,
                ChannelCredentials.Insecure,
                new[] { keepAliveTime, keepAliveTimeout, keepAliveWithoutCalls });
            return new Health.HealthClient(channel);
        }
    }
}