// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Concepts;

namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Represents the host of a <see cref="Microservice" />.
    /// </summary>
    public class MicroserviceHost : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string" /> to <see cref="MicroserviceHost" />.
        /// </summary>
        /// <param name="host">The host of a microservice.</param>
        public static implicit operator MicroserviceHost(string host) => new MicroserviceHost { Value = host };
    }
}