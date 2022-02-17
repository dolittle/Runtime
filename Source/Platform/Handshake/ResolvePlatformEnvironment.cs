// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Platform;
using Dolittle.Runtime.DependencyInversion;


namespace Dolittle.Runtime.Platform.Handshake;

/// <summary>
/// Represents an implementation of <see cref="IResolvePlatformEnvironment"/>.
/// </summary>
[Singleton]
public class ResolvePlatformEnvironment : IResolvePlatformEnvironment
{
    readonly PlatformConfiguration _platformConfig;

    public ResolvePlatformEnvironment(PlatformConfiguration platformConfig)
    {
        _platformConfig = platformConfig;
    }

    /// <inheritdoc />
    public Task<PlatformEnvironment> Resolve()
    {
        return Task.FromResult(new PlatformEnvironment(
            _platformConfig.CustomerID,
            _platformConfig.ApplicationID,
            _platformConfig.MicroserviceID,
            _platformConfig.CustomerName,
            _platformConfig.ApplicationName,
            _platformConfig.MicroserviceName,
            _platformConfig.Environment));
    }
}
