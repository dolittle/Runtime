// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Lifecycle;

namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Represents an implementation of <see cref="IMicroservices" />.
    /// </summary>
    [Singleton]
    public class Microservices : IMicroservices
    {
        readonly MicroservicesConfiguration _microservicesConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Microservices"/> class.
        /// </summary>
        /// <param name="microservicesConfiguration">The <see cref="MicroservicesConfiguration" />.</param>
        public Microservices(MicroservicesConfiguration microservicesConfiguration)
        {
            _microservicesConfiguration = microservicesConfiguration;
        }

        /// <inheritdoc/>
        public MicroserviceAddress GetAddressFor(Microservice microservice)
        {
            if (!_microservicesConfiguration.TryGetValue(microservice, out var address)) throw new NoConfiguredAddressForMicroservice(microservice);
            return address;
        }
    }
}