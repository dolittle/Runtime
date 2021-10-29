// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes
{
    /// <summary>
    /// Represents an implementation of <see cref="IDiscoverEventTypes"/>.
    /// </summary>
    public class DiscoverEventTypes : IDiscoverEventTypes
    {
        readonly IManagementClient _client;

        public DiscoverEventTypes(IManagementClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public Task<IEnumerable<EventType>> Discover(MicroserviceAddress runtime)
            => _client.GetAll(runtime);
    }
}
