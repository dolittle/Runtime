// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Domain.Platform;
using Dolittle.Runtime.Rudimentary;

namespace Microservices;

/// <summary>
/// Represents the host of a <see cref="MicroserviceId" />.
/// </summary>
/// <param name="Value">The host of a microservice.</param>
public record MicroserviceHost(string Value) : ConceptAs<string>(Value)
{
    /// <summary>
    /// Implicitly convert from <see cref="string" /> to <see cref="MicroserviceHost" />.
    /// </summary>
    public static implicit operator MicroserviceHost(string host) => new(host);
}
