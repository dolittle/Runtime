/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Services;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindApplicationServices"/> for exposing
    /// management service implementations for DependencyInversion
    /// </summary>
    public class ApplicationServices : ICanBindApplicationServices
    {
        readonly ClientsService _clientService;

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationServices"/>
        /// </summary>
        /// <param name="clientService">Instance of <see cref="ClientsService"/></param>
        public ApplicationServices(ClientsService clientService)
        {
            _clientService = clientService;
        }

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[] {
                new Service(_clientService, Grpc.Server.Clients.BindService(_clientService), Grpc.Server.Clients.Descriptor)
            };
        }
    }
}