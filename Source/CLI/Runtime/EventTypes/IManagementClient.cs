// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes
{
    /// <summary>
    /// Defines the EventTypes management client.
    /// </summary>
    public interface IManagementClient
    {
        
        /// <summary>
        /// Gets all registered Event Types.
        /// </summary>
        /// <param name="runtime">The address of the Runtime to connect to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<Dolittle.Runtime.Events.EventType>> GetAll(MicroserviceAddress runtime);
    }
}
