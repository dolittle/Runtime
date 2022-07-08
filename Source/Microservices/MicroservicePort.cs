// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Rudimentary;

namespace Microservices;

/// <summary>
/// Represents the port of a <see cref="MicroserviceId" />.
/// </summary>
/// <param name="Value">The port of a microservice.</param>
public record MicroservicePort(int Value) : ConceptAs<int>(Value)
{
    /// <summary>
    /// Implicitly convert from <see cref="int" /> to <see cref="MicroservicePort" />.
    /// </summary>
    /// <param name="port">The port of a microservice.</param>
    public static implicit operator MicroservicePort(int port) => new(port);
}
