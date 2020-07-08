// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ApplicationModel;
using Dolittle.Concepts;

namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Represents the port of a <see cref="Microservice" />.
    /// </summary>
    public class MicroservicePort : ConceptAs<int>
    {
        /// <summary>
        /// Implicitly convert from <see cref="int" /> to <see cref="MicroservicePort" />.
        /// </summary>
        /// <param name="port">The port of a microservice.</param>
        public static implicit operator MicroservicePort(int port) => new MicroservicePort { Value = port };
    }
}