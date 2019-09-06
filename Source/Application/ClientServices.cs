/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Dolittle.Types;
using Grpc.Core;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientServices : IClientServices
    {
        readonly IEnumerable<ClientServiceDefinition> _services;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientServicesDefiners"></param>
        public ClientServices(IInstancesOf<IDefineApplicationClientServices> clientServicesDefiners)
        {
            _services = clientServicesDefiners.SelectMany(_ => _.Services).ToArray();
        }

        /// <inheritdoc/>
        public T GetService<T>() where T : ClientBase<T>
        {
            throw new System.NotImplementedException();
        }
    }
}