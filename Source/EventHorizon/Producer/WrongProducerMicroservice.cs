// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Exception that gets thrown when the wrong microservice has received an event horizon subscription.
    /// </summary>
    public class WrongProducerMicroservice : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongProducerMicroservice"/> class.
        /// </summary>
        /// <param name="producerMicroservice">The producer <see cref="Microservice" />.</param>
        public WrongProducerMicroservice(Microservice producerMicroservice)
            : base($"Expected microservice to be '{producerMicroservice}'")
        {
        }
    }
}
