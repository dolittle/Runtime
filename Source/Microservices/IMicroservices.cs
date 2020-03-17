// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;

namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Defines a system that knows about the configured microservices.
    /// </summary>
    public interface IMicroservices
    {
        /// <summary>
        /// Gets the <see cref="MicroserviceAddress" /> for a given <see cref="Microservice" />.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <returns>THe <see cref="MicroserviceAddress" /> for the given <see cref="Microservice" />.</returns>
        MicroserviceAddress GetAddressFor(Microservice microservice);
    }
}