// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;

namespace Dolittle.Runtime.Microservices;

/// <summary>
/// Represents the address of a <see cref="Microservice" />.
/// </summary>
/// <param name="Host">The host of a microservice.</param>
/// <param name="Port">The host of a microservice.</param>
public record MicroserviceAddress(MicroserviceHost Host, MicroservicePort Port);

/// <summary>
/// Represents the address configuration of a <see cref="Microservice" />.
/// </summary>
/// <param name="Host">The host of a microservice.</param>
/// <param name="Port">The host of a microservice.</param>
public record MicroserviceAddressConfiguration(string Host, int Port)
{
    public static implicit operator MicroserviceAddress(MicroserviceAddressConfiguration config) => new(config.Host, config.Port);
}