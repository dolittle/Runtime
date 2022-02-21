// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Runtime.Configuration;
using Dolittle.Runtime.Services;

namespace Services.Clients;

/// <summary>
/// Represents the configuration for clients by their <see cref="EndpointVisibility"/>.
/// </summary>
[Configuration("clients")]
public class ClientEndpointsConfiguration : ReadOnlyDictionary<EndpointVisibility, ClientEndpointConfiguration>
{
    public ClientEndpointsConfiguration(IDictionary<EndpointVisibility, ClientEndpointConfiguration> dictionary)
        : base(dictionary)
    {
    }
}
