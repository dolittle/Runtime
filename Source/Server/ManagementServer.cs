/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Execution;
using Dolittle.Types;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents an implementation of <see cref="IManagementServer"/>
    /// </summary>
    [Singleton]
    public class ManagementServer : IManagementServer
    {
        readonly IInstancesOf<ICanBindManagementServices> _services;

        /// <summary>
        /// Initializes a new instance of <see cref="ManagementServer"/>
        /// </summary>
        /// <param name="services"><see cref="IInstancesOf{ICanBindManagementServices}">Binders of management services</see></param>
        public ManagementServer(IInstancesOf<ICanBindManagementServices> services)
        {
            _services = services;
        }

        /// <inheritdoc/>
        public void Start()
        {
            
        }
    }
}