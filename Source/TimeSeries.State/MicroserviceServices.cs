// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Microservices;
using Dolittle.Services;
using grpc = Dolittle.TimeSeries.State.Microservice;

namespace Dolittle.Runtime.TimeSeries.State
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindMicroserviceServices"/> - providing
    /// microservice services for inter microservice communication.
    /// </summary>
    public class MicroserviceServices : ICanBindMicroserviceServices
    {
        readonly DataPointsStateService _dataPointsStateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceServices"/> class.
        /// </summary>
        /// <param name="dataPointsStateService">Instance of <see cref="DataPointsStateService"/>.</param>
        public MicroserviceServices(DataPointsStateService dataPointsStateService)
        {
            _dataPointsStateService = dataPointsStateService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "TimeSeries";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_dataPointsStateService, grpc.DataPointsState.BindService(_dataPointsStateService), grpc.DataPointsState.Descriptor)
            };
        }
    }
}