// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Metrics.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace Dolittle.Runtime.Server.Web;

[DisableAutoRegistration]
public class KestrelConfiguration : IConfigureOptions<KestrelServerOptions>
{
    readonly WebServerConfiguration _configuration;
    
    public KestrelConfiguration(IOptions<WebServerConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    /// <inheritdoc />
    public void Configure(KestrelServerOptions options)
    {
        options.ListenAnyIP(_configuration.Port);
    }
}
