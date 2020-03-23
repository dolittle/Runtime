// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using Dolittle.Runtime.Management;
using Dolittle.Services;
using grpc = contracts::Dolittle.Runtime.Metrics.Management;

namespace Dolittle.Runtime.Metrics.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindManagementServices"/> for exposing
    /// management service implementations for Heads.
    /// </summary>
    public class ManagementServices : ICanBindManagementServices
    {
        readonly MetricsService _metricsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementServices"/> class.
        /// </summary>
        /// <param name="metricsService"><see cref="MetricsService"/> to expose.</param>
        public ManagementServices(MetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Metrics";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_metricsService, grpc.Metrics.BindService(_metricsService), grpc.Metrics.Descriptor)
            };
        }
    }
}