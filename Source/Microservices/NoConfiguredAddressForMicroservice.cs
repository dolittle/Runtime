// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;

namespace Dolittle.Runtime.Microservices
{
    /// <summary>
    /// Exception that gets thrown when there is no address configured for the given <see cref="Microservice" />.
    /// </summary>
    public class NoConfiguredAddressForMicroservice : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoConfiguredAddressForMicroservice"/> class.
        /// </summary>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        public NoConfiguredAddressForMicroservice(Microservice microservice)
            : base($"There is no address configured for microservice '{microservice}'")
        {
        }
    }
}