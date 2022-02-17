// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Clients;

/// <summary>
/// Represents the configuration for clients by their <see cref="EndpointVisibility"/>.
/// </summary>
[Name("clients")]
public class ClientEndpointsConfiguration : Dictionary<EndpointVisibility, ClientEndpointConfiguration>, IConfigurationObject
{
}
