// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using Dolittle.Runtime.Management;
using Dolittle.Services;
using grpc = contracts::Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Services.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindManagementServices"/> for exposing
    /// management service implementations for DependencyInversion.
    /// </summary>
    public class ManagementServices : ICanBindManagementServices
    {
        readonly BoundServicesService _boundServicesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementServices"/> class.
        /// </summary>
        /// <param name="boundServicesService">The <see cref="BoundServicesService"/>.</param>
        public ManagementServices(BoundServicesService boundServicesService)
        {
            _boundServicesService = boundServicesService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Services";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new[]
            {
                new Service(_boundServicesService, grpc.BoundServices.BindService(_boundServicesService), grpc.BoundServices.Descriptor)
            };
        }
    }
}