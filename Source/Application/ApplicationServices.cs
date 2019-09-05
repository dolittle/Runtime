/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Hosting;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanBindApplicationServices"/> for exposing
    /// management service implementations for DependencyInversion
    /// </summary>
    public class ApplicationServices : ICanBindApplicationServices
    {
        readonly ClientService _clientService;

        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationServices"/>
        /// </summary>
        /// <param name="clientService">Instance of <see cref="ClientService"/></param>
        public ApplicationServices(ClientService clientService)
        {
            _clientService = clientService;
        }

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            return new Service[] {
                new Service(Grpc.Client.BindService(_clientService), Grpc.Client.Descriptor)
            };
        }
    }    
}