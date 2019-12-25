// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Management;
using Dolittle.Services;

namespace Dolittle.Runtime.Heads.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindManagementServices"/> for exposing
    /// management service implementations for Heads.
    /// </summary>
    public class ManagementServices : ICanBindManagementServices
    {
        readonly HeadsService _clientsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementServices"/> class.
        /// </summary>
        /// <param name="clientsService"><see cref="HeadsService"/> to expose.</param>
        public ManagementServices(HeadsService clientsService)
        {
            _clientsService = clientsService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Runtime";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[]
            {
                new Service(_clientsService, Dolittle.Heads.Management.Heads.BindService(_clientsService), Dolittle.Heads.Management.Heads.Descriptor)
            };
        }
    }
}