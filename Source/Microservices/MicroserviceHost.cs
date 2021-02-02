// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.ApplicationModel;
namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Represents the host of a <see cref="Microservice" />.
    /// </summary>
    /// <param name="host">The host of a microservice.</param>
    public record MicroserviceHost(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicitly convert from <see cref="string" /> to <see cref="MicroserviceHost" />.
        /// </summary>
        public static implicit operator MicroserviceHost(string host) => new(host);
    }
}