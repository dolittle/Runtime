/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using Dolittle.Lifecycle;
using Dolittle.Types;

namespace Dolittle.Runtime.Applications
{
    /// <summary>
    /// Represents an implementation of <see cref="IApplicationServices"/>
    /// </summary>
    [Singleton]
    public class ApplicationServices : IApplicationServices
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApplicationServices"/>
        /// </summary>
        /// <param name="definers">Instances of <see cref="IDefineApplicationServices">definers</see></param>
        public ApplicationServices(IInstancesOf<IDefineApplicationServices> definers)
        {
            Services = definers.SelectMany(_ => _.Services);
        }

        /// <inheritdoc/>
        public IEnumerable<ClientServiceDefinition> Services {  get; }
    }
}