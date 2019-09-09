/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Dolittle.Lifecycle;
using Dolittle.Types;

namespace Dolittle.Runtime.Application
{
    /// <summary>
    /// Represents an implementation of <see cref="IApplicationClientServices"/>
    /// </summary>
    [Singleton]
    public class ApplicationClientServices : IApplicationClientServices
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationClientServices"/>
        /// </summary>
        /// <param name="definers">Instances of <see cref="IDefineApplicationClientServices">definers</see></param>
        public ApplicationClientServices(IInstancesOf<IDefineApplicationClientServices> definers)
        {
            Services = definers.SelectMany(_ => _.Services);
        }

        /// <inheritdoc/>
        public IEnumerable<ClientServiceDefinition> Services {  get; }
    }
}