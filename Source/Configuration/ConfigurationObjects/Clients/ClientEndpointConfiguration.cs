// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Configuration.ConfigurationObjects.Clients;

/// <summary>
/// Represents the configuration typically used to connect clients to a host.
/// </summary>
public record ClientEndpointConfiguration(string Host, int Port);