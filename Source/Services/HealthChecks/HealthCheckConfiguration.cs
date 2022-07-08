// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Services.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Services.HealthChecks;

/// <summary>
/// Represents an implementation of <see cref="IConfigureOptions{TOptions}"/> for <see cref="HealthCheckServiceOptions"/> that adds a <see cref="HealthCheckRegistration"/> for a specific <see cref="EndpointVisibility"/>.
/// </summary>
[DisableAutoRegistration]
public class HealthCheckConfiguration : IConfigureOptions<HealthCheckServiceOptions>
{
    readonly EndpointVisibility _visibility;
    readonly Func<IOptions<EndpointsConfiguration>> _getConfiguration;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckConfiguration"/> class.
    /// </summary>
    /// <param name="visibility">The endpoint visibility to add a health check for.</param>
    /// <param name="getConfiguration">The factory to use to get the endpoint configuration.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public HealthCheckConfiguration(EndpointVisibility visibility, Func<IOptions<EndpointsConfiguration>> getConfiguration, ILogger logger)
    {
        _visibility = visibility;
        _getConfiguration = getConfiguration;
        _logger = logger;
    }

    /// <inheritdoc />
    public void Configure(HealthCheckServiceOptions options)
    {
        _logger.AddingHealthCheckFor(_visibility);
        options.Registrations.Add(new HealthCheckRegistration(
            $"{nameof(EndpointHealthCheck)}[{_visibility}]",
            new EndpointHealthCheck(_visibility, _getConfiguration),
            null,
            null));
    }
}
