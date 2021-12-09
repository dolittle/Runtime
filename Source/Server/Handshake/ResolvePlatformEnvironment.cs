// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.ResourceTypes.Configuration;
using Dolittle.Runtime.Server.Configuration;

namespace Dolittle.Runtime.Server.Handshake;

/// <summary>
/// Represents an implementation of <see cref="IResolvePlatformEnvironment"/>.
/// </summary>
[Singleton]
public class ResolvePlatformEnvironment : IResolvePlatformEnvironment
{
    readonly IConfigurationFor<PlatformConfiguration> _platformConfig;

    public ResolvePlatformEnvironment(IConfigurationFor<PlatformConfiguration> platformConfig)
    {
        _platformConfig = platformConfig;
    }

    /// <inheritdoc />
    public Task<PlatformEnvironment> Resolve()
    {
        var config = _platformConfig.Instance;
        return Task.FromResult(new PlatformEnvironment(config.MicroserviceID, config.Environment));
    }
}
