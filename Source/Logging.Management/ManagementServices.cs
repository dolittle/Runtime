// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Collections.Generic;
using Dolittle.Runtime.Management;
using Dolittle.Services;
using grpc = contracts::Dolittle.Runtime.Logging.Management;

namespace Dolittle.Runtime.Logging.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindManagementServices"/> for exposing
    /// management service implementations for Logs.
    /// </summary>
    public class ManagementServices : ICanBindManagementServices
    {
        readonly LogService _logService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementServices"/> class.
        /// </summary>
        /// <param name="logService"><see cref="LogService"/> to expose.</param>
        public ManagementServices(LogService logService)
        {
            _logService = logService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Runtime";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_logService, grpc.Log.BindService(_logService), grpc.Log.Descriptor)
            };
        }
    }
}