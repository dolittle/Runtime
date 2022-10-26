// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Services.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Services.Hosting;

[DisableAutoRegistration]
public class KestrelConfiguration : IConfigureOptions<KestrelServerOptions>
{
    readonly EndpointConfiguration _configuration;
    readonly HttpProtocols _protocols;

    public KestrelConfiguration(IOptions<EndpointsConfiguration> configuration, EndpointVisibility visibility, HttpProtocols protocols)
    {
        _configuration = configuration.Value.GetConfigurationFor(visibility);
        _protocols = protocols;
    }

    /// <inheritdoc />
    public void Configure(KestrelServerOptions options)
    {
        options.ListenAnyIP(
            _configuration.Port,
            _ =>
            {
                _.Protocols = _protocols;
            });
    }
}
