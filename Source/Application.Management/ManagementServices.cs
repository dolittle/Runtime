/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Management;
using Dolittle.Services;

namespace Dolittle.Runtime.Application.Management
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindManagementServices"/> for exposing
    /// management service implementations for Applications
    /// </summary>
    public class ManagementServices : ICanBindManagementServices
    {
        readonly ClientsService _clientsService;

        /// <summary>
        /// Initializes a new instance of <see cref="ManagementServices"/>
        /// </summary>
        /// <param name="clientsService"><see cref="ClientsService"/> to expose</param>
        public ManagementServices(ClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Application";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[] {
                new Service(_clientsService, Dolittle.Application.Management.Clients.BindService(_clientsService), Dolittle.Application.Management.Clients.Descriptor)
            };
        }
    }
}