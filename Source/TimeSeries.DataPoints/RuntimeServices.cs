// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using Dolittle.Runtime.Services;
using Dolittle.Services;
using grpc = contracts::Dolittle.Runtime.TimeSeries.DataPoints;

namespace Dolittle.Runtime.TimeSeries.DataPoints
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> - providing runtime services
    /// for working with datapoints.
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly DataPointProcessorsService _dataPointProcessorsService;
        readonly DataPointStreamService _dataPointStreamService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeServices"/> class.
        /// </summary>
        /// <param name="dataPointProcessorsService"><see cref="DataPointProcessorsService"/> service.</param>
        /// <param name="dataPointStreamService"><see cref="DataPointStreamService"/> service.</param>
        public RuntimeServices(
            DataPointProcessorsService dataPointProcessorsService,
            DataPointStreamService dataPointStreamService)
        {
            _dataPointProcessorsService = dataPointProcessorsService;
            _dataPointStreamService = dataPointStreamService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "TimeSeries";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_dataPointProcessorsService, grpc.DataPointProcessors.BindService(_dataPointProcessorsService), grpc.DataPointProcessors.Descriptor),
                new Service(_dataPointStreamService, grpc.DataPointStream.BindService(_dataPointStreamService), grpc.DataPointStream.Descriptor),
            };
        }
    }
}