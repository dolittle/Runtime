// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.EventHorizon.Producer;
using Dolittle.Runtime.Microservices;
using Dolittle.Services;

namespace Dolittle.Runtime.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindMicroserviceServices"/> for exposing
    /// microservice service implementations.
    /// </summary>
    public class MicroserviceServices : ICanBindMicroserviceServices
    {
        readonly ConsumerService _consumerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceServices"/> class.
        /// </summary>
        /// <param name="consumerService">The <see cref="ConsumerService" />.</param>
        public MicroserviceServices(ConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "EventHorizon";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices() =>
            new Service[]
            {
                new Service(_consumerService, Contracts.Consumer.BindService(_consumerService), Contracts.Consumer.Descriptor)
            };
    }
}