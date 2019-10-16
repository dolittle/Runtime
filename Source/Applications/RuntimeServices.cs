/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Services;

namespace Dolittle.Runtime.Applications
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindRuntimeServices"/> for exposing
    /// management service implementations for DependencyInversion
    /// </summary>
    public class RuntimeServices : ICanBindRuntimeServices
    {
        readonly ClientsService _clientService;

        /// <summary>
        /// Initializes a new instance of <see cref="RuntimeServices"/>
        /// </summary>
        /// <param name="clientService">Instance of <see cref="ClientsService"/></param>
        public RuntimeServices(ClientsService clientService)
        {
            _clientService = clientService;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Application";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[] {
                new Service(_clientService, Dolittle.Applications.Runtime.Clients.BindService(_clientService), Dolittle.Applications.Runtime.Clients.Descriptor)
            };
        }
    }
}