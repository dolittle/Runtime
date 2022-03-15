// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Services.HealthChecks;
using Dolittle.Runtime.Types;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Server.HealthChecks
{
    /// <summary>
    /// Represents an implementation of <see cref="IConfigureOptions{TOptions}"/> for <see cref="HealthCheckServiceOptions"/> that automatically adds all implemented instances of <see cref="IHealthCheck"/> by type name.
    /// </summary>
    public class HealthCheckConfiguration : IConfigureOptions<HealthCheckServiceOptions>
    {
        readonly IEnumerable<IHealthCheck> _healthChecks;
        readonly IEndpoints _endpoints;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckConfiguration"/> class.
        /// </summary>
        /// <param name="healthChecks">The instances that implement <see cref="IHealthCheck"/> to add as registrations in the configuration.</param>
        /// <param name="logger">The logger to use for logging.</param>
        public HealthCheckConfiguration(IInstancesOf<IHealthCheck> healthChecks, IEndpoints endpoints, ILogger logger)
        {
            _healthChecks = healthChecks.Where(_ => _.GetType() != typeof(EndpointHealthCheck));
            _endpoints = endpoints;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Configure(HealthCheckServiceOptions options)
        {
            foreach (var healthCheck in _healthChecks)
            {
                var registration = new HealthCheckRegistration(healthCheck.GetType().Name, healthCheck, null, null);
                options.Registrations.Add(registration);
            }
            foreach (var endpoint in _endpoints.GetEndpoints())
            {
                options.Registrations.Add(new HealthCheckRegistration(
                    $"{nameof(EndpointHealthCheck)}[{Enum.GetName(typeof(EndpointVisibility), endpoint.Visibility)}]",
                    new EndpointHealthCheck(endpoint.Configuration),
                    null,
                    null));
            }
        }
    }
}

